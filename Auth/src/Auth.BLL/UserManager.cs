﻿using System;
using System.Threading.Tasks;
using Auth.BLL.Interfaces;
using Auth.BLL.Models;
using Auth.DAL;
using Auth.DAL.Entities;
using IdentityModel;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;

namespace Auth.BLL
{
    public class UserManager : IUserManager
    {
        private readonly AuthContext context;
        private readonly ICrypto crypto;

        public UserManager(AuthContext context, ICrypto crypto)
        {
            this.context = context;
            this.crypto = crypto;
        }

        public async Task Register(UserModel model)
        {
            LocalUser user = await this.CreateUser(model);
            await this.AddRoleClaim(user);
        }

        private async Task<LocalUser> CreateUser(UserModel model)
        {
            string salt = this.crypto.GenerateSalt();

            var localUser = new LocalUser
            {
                Email = model.Email,
                Password = this.crypto.ComputeHash(model.Password, salt),
                Salt = salt
            };

            await this.context.LocalUsers.AddAsync(localUser);
            await this.context.SaveChangesAsync();

            return localUser;
        }

        private async Task AddRoleClaim(LocalUser user)
        {
            Claim claim = await this.context.Claims.FirstAsync(x => x.Type == JwtClaimTypes.Role && x.Value == Roles.User);

            var localUserClaim = new LocalUserClaim
            {
                LocalUserId = user.Id,
                LocalUser = user,
                ClaimId = claim.Id,
                Claim = claim
            };

            await this.context.LocalUserClaims.AddAsync(localUserClaim);
            await this.context.SaveChangesAsync();
        }

        public async Task<UserValidationModel> VerifyUser(string email, string password)
        {
            var model = new UserValidationModel();

            LocalUser user = await this.context.LocalUsers
                                                .Include(x => x.LocalUserClaims)
                                                .ThenInclude(x => x.Claim)
                                                .FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                string hash = this.crypto.ComputeHash(password, user.Salt);
                if (hash == user.Password)
                {
                    model.IsVerified = true;
                    model.User = user;
                }
            }

            return model;
        }

        public async Task AddExternalUser(ExternalUser externalUser)
        {
            var existLocalUser = await this.context.LocalUsers.FirstOrDefaultAsync(x => x.Email == externalUser.Email);
            if (existLocalUser != null)
            {
                if (!await this.context.ExternalUsers.AnyAsync(x => x.Email == externalUser.Email))
                {
                    await this.AddExternalToLocalUser(externalUser, existLocalUser);
                }
            }
            else
            {
                await this.AddExistLocalUserAndExternal(externalUser);
            }
        }

        private async Task AddExternalToLocalUser(ExternalUser externalUser, LocalUser existLocalUser)
        {
            externalUser.User = existLocalUser;
            await this.context.ExternalUsers.AddAsync(externalUser);
            await this.context.SaveChangesAsync();
        }

        private async Task AddExistLocalUserAndExternal(ExternalUser externalUser)
        {
            var localUser = await this.CreateUser(new UserModel() { Email = externalUser.Email });
            await this.AddRoleClaim(localUser);
            externalUser.User = localUser;
            await this.context.ExternalUsers.AddAsync(externalUser);
            await this.context.SaveChangesAsync();
        }
    }
}