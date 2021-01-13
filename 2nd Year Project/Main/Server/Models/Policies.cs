using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace EduLocate.Server.Models
{
    /// <summary>Statically-accessible policy information.</summary>
    public static class Policies
    {
        /// <summary>The name of the UserManager claim.</summary>
        public const string UserManagerClaimName = "UserManager";

        /// <summary>The name of the DataManager claim.</summary>
        public const string DataManagerClaimName = "DataManager";


        /// <summary>The name of the UserManager policy.</summary>
        public const string UserManagerPolicyName = "UserManagerPolicy";

        /// <summary>The name of the DataManager policy.</summary>
        public const string DataManagerPolicyName = "DataManagerPolicy";

        /// <summary>Generates an enumerable of policies to use for authorisation. Generated using the policy names above.</summary>
        /// <returns>An enumerable of authorisation policies based on the claim/policy names defined in <see cref="Policies"/>.</returns>
        public static IEnumerable<(string, Action<AuthorizationPolicyBuilder>)> GenerateAuthorisationPolicies()
        {
            yield return (UserManagerPolicyName,
                policy => policy.RequireAssertion(ctx => HasUserManagerClaim(ctx.User.Claims)));
            yield return (DataManagerPolicyName,
                policy => policy.RequireAssertion(ctx => HasDataManagerClaim(ctx.User.Claims)));
        }

        /// <summary>Helper to determine if a set of claims contains the UserManager claim.</summary>
        /// <param name="claims">The claims to check in.</param>
        /// <returns>True if the claims contains the UserManager claim.</returns>
        public static bool HasUserManagerClaim(IEnumerable<Claim> claims)
        {
            return claims.Any(claim => claim.Type == UserManagerClaimName && claim.Value == "true");
        }

        /// <summary>Helper to determine if a set of claims contains the DataManager claim.</summary>
        /// <param name="claims">The claims to check in.</param>
        /// <returns>True if the claims contains the DataManager claim.</returns>
        public static bool HasDataManagerClaim(IEnumerable<Claim> claims)
        {
            return claims.Any(claim => claim.Type == DataManagerClaimName && claim.Value == "true");
        }
    }
}