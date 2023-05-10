using System.Collections.Generic;
using UnityEngine;

namespace Game.Manager
{
    public class AvatarManager
    {
        private static Dictionary<string, Texture2D> localAvatarCache = new Dictionary<string, Texture2D>();
        public static void SetLocalAvatars(IList<Texture2D> avatars)
        {
            foreach (var avatar in avatars)
            {
                localAvatarCache[avatar.name] = avatar;
            }
        }

        public static Texture2D GetLocalAvatarById(int avatarId)
        {
            localAvatarCache.TryGetValue($"{avatarId}", out var avatar);
            return avatar;
        }
    }
}