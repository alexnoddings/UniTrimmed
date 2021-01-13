using System;
using System.Collections.Generic;
using System.Linq;
using AlexNoddings.Protocols.Core;
using AlexNoddings.Protocols.Runtime.Internal;

namespace AlexNoddings.Protocols.Runtime.Extensions
{
    public static partial class KnowledgeExtensions
    {
        public static IKnowledge Stripped(this IKnowledge knowledge, ICollection<IKnowledge> keys)
        {
            if (knowledge == null) throw new ArgumentNullException(nameof(knowledge));
            if (keys == null) throw new ArgumentNullException(nameof(keys));

            // No key present
            if (knowledge.Key == null || knowledge.Key.CanCreateFrom(keys))
            {
                if (knowledge.Parts.Count > 1)
                    return new SimpleKnowledge(null, knowledge.Parts.Select(p => p.Stripped(keys)));
                return knowledge.Parts.First().Stripped(keys);
            }

            // If the key cannot be stripped, none of its parts may be
            return knowledge;
        }

        public static bool CanCreateFrom(this IKnowledge knowledge, ICollection<IKnowledge> keys)
        {
            if (knowledge == null) throw new ArgumentNullException(nameof(knowledge));
            if (keys == null) throw new ArgumentNullException(nameof(keys));

            if (knowledge.Key == null)
            {
                return knowledge.Parts.All(k => k.CanCreateFrom(keys));
            }
            return knowledge.Key.CanCreateFrom(keys) && knowledge.Parts.All(k => k.CanCreateFrom(keys));
        }

        public static bool CanCreateFromExcept(this IKnowledge knowledge, ICollection<IKnowledge> keys)
        {
            if (knowledge == null) throw new ArgumentNullException(nameof(knowledge));
            if (keys == null) throw new ArgumentNullException(nameof(keys));

            var otherKeys = keys.Where(k => !k.Equals(knowledge)).ToList();
            return knowledge.CanCreateFrom(otherKeys);
        }
    }
}
