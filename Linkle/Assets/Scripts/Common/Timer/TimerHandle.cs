using System;

namespace Common.Timer
{
    public class TimerHandle : IEquatable<TimerHandle>
    {
        public static TimerHandle Next(int group)
        {
            if (long.MaxValue == handleAllocator)
            {
                handleAllocator = 0;
            }
            return new TimerHandle(group, ++handleAllocator);
        }
        
        private static long handleAllocator = 0;
        
        public readonly long Handle;
        public readonly int Group;

        public bool IsValid => value > 0;

        private long value;

        private TimerHandle(int group, long handle)
        {
            Group = group;
            Handle = handle;
            value = Handle;
        }

        public void Invalidate()
        {
            value = 0;
        }

        public override bool Equals(object other) => other is TimerHandle other1 && this.Equals(other1);

        public bool Equals(TimerHandle other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return Handle == other.Handle && Group == other.Group;
        }
        
        public override int GetHashCode()
        {
            var hashCode = Group.GetHashCode();
            var num = Handle.GetHashCode() << 2;
            return hashCode ^ num;
        }
        
        public override string ToString() => $"{Group},{Handle}";

        public static bool operator ==(TimerHandle lhs, TimerHandle rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                return ReferenceEquals(rhs, null);
            }
            else if (ReferenceEquals(rhs, null))
            {
                return false;
            }
            return lhs.Handle == rhs.Handle && lhs.Group == rhs.Group;
        }
        
        public static bool operator !=(TimerHandle lhs, TimerHandle rhs) => !(lhs == rhs);
    }
}