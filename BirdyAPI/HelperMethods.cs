using BirdyModel.Attributes;
using BirdyModel.Birdy;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BirdyAPI
{
    public static class HelperMethods
    {
        public static string GetCallerFromToken(System.Security.Principal.IIdentity pIdentity)
        {
            var loUser = "";

            if (pIdentity is ClaimsIdentity identity)
            {
                loUser = identity.Claims.FirstOrDefault(x => x.Type.Contains("name"))?.Value;

            }
            return loUser;
        }

        public static Guid GetApiUserIdFromToken(System.Security.Principal.IIdentity pIdentity)
        {
            var loUserId = new Guid();

            if (pIdentity is ClaimsIdentity identity)
            {
                var loTemp = identity.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))?.Value;
                if (!Guid.TryParse(loTemp, out loUserId))
                {
                    loUserId = Guid.Empty;
                }

            }
            return loUserId;
        }

        public static int GetUserTypeFromToken(System.Security.Principal.IIdentity pIdentity)
        {
            var loUserType = -1;

            if (!(pIdentity is ClaimsIdentity identity)) return loUserType;

            var loTemp = identity.Claims.FirstOrDefault(x => x.Type.ToLower().Contains("gender"))?.Value;
            if (!int.TryParse(loTemp, out loUserType))
            {
                loUserType = -1;
            }
            return loUserType;
        }

        public static string SerializePhone(string pPhone)
        {
            if (string.IsNullOrEmpty(pPhone))
            {
                return "";
            }

            pPhone = pPhone.Replace(")", "");
            pPhone = pPhone.Replace("(", "");
            pPhone = pPhone.Replace("-", "");
            pPhone = pPhone.Replace(" ", "");

            if (pPhone.StartsWith("+90"))
                return pPhone;
            if (pPhone.StartsWith("90") && pPhone.Length == 12)
                return "+" + pPhone;
            if (pPhone.StartsWith("0") && pPhone.Length == 11)
                return "+9" + pPhone;

            return "+90" + pPhone;
        }

        public static string GenerateToken(string pUserId, int pUserType)
        {
            var someClaims = new[]{
                new Claim(JwtRegisteredClaimNames.NameId,pUserId),
                new Claim(JwtRegisteredClaimNames.Birthdate,DateTime.Now.ToShortDateString()),
                new Claim(JwtRegisteredClaimNames.Gender,pUserType.ToString()),
            };

            var token = new JwtSecurityToken
            (
                "MesnetBilisim2021",
                "2020Winvestate-Api",
                someClaims,
                expires: DateTime.UtcNow.AddDays(30), // 30 gün geçerli olacak,
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("2020 MesnetBilisimSigningCridential For Winvestate Api")),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static List<ChangeLog> TrackChangesOfObject(object newEntry, object oldEntry)
        {
            List<ChangeLog> logs = new();

            var loCheckAttribute = Attribute.GetCustomAttribute(newEntry.GetType(), typeof(IgnoreLoggingAttribute));

            if (loCheckAttribute != null)
            {
                return logs;
            }

            if (oldEntry == null || newEntry == null)
            {
                return logs;
            }

            var oldType = oldEntry.GetType().BaseType.Name.ToString().ToLower() is "object" or "entity" ? oldEntry.GetType() : oldEntry.GetType().BaseType;
            var newType = newEntry.GetType().BaseType.Name.ToString().ToLower() is "object" or "entity" ? newEntry.GetType() : newEntry.GetType().BaseType;

            if (oldType != newType)
            {
                return logs; //Types don't match, cannot log changes
            }

            var oldProperties = oldType.GetProperties();
            var newProperties = newType.GetProperties();

            var dateChanged = DateTime.Now;
            var primaryKey = oldProperties.Where(x => Attribute.IsDefined(x, typeof(LoggingPrimaryKeyAttribute))).First().GetValue(oldEntry)?.ToString();
            var primaryKeyField = oldProperties.Where(x => Attribute.IsDefined(x, typeof(LoggingPrimaryKeyAttribute))).First()?.Name.ToString();
            var loTableName = (TableAttribute)Attribute.GetCustomAttribute(newEntry.GetType(), typeof(TableAttribute));

            var loUserId = newProperties.FirstOrDefault(x => x.Name == "row_update_user")?.GetValue(newEntry);

            if (!Guid.TryParse(loUserId?.ToString(), out Guid loUser))
            {
                loUser = Guid.Empty;
            }

            foreach (var oldProperty in oldProperties)
            {
                var matchingProperty = newProperties.Where(x => !Attribute.IsDefined(x, typeof(IgnoreLoggingAttribute))
                                                                && x.Name == oldProperty.Name
                                                                && x.PropertyType == oldProperty.PropertyType)
                                                    .FirstOrDefault();
                if (matchingProperty == null)
                {
                    continue;
                }
                var oldValueTemp = oldProperty.GetValue(oldEntry)?.ToString();
                var newValueTemp = matchingProperty.GetValue(newEntry)?.ToString();
                var loResult = oldValueTemp == newValueTemp;

                if (oldProperty.PropertyType.ToString().ToLower().Contains("decimal"))
                {
                    loResult = Convert.ToDecimal(oldValueTemp) == Convert.ToDecimal(newValueTemp);
                }

                if (matchingProperty != null && !loResult)
                {
                    logs.Add(new ChangeLog()
                    {
                        primary_key = primaryKey,
                        date_changed = dateChanged,
                        class_name = loTableName.Name,
                        property_name = matchingProperty.Name,
                        old_value = oldProperty.GetValue(oldEntry) == null ? "-" : oldProperty.GetValue(oldEntry).ToString(),
                        new_value = matchingProperty.GetValue(newEntry) == null ? "-" : matchingProperty.GetValue(newEntry).ToString(),
                        changed_user = loUser,
                        primary_key_field = primaryKeyField,
                        operation = "UPDATE"
                    });
                }
            }

            return logs;
        }
    }
}
