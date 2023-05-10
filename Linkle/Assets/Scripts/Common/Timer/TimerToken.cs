using System;

namespace Common.Timer
{
    public class TimerToken : IEquatable<TimerToken>
    {
        public readonly TimerHandle Handle;
        
        public TimerToken(TimerHandle handle)
        {
            Handle = handle;
        }
        
        public override bool Equals(object other) => other is TimerToken other1 && this.Equals(other1);

        public bool Equals(TimerToken other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return Handle == other.Handle;
        }
        
        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }
        
        public override string ToString() => $"{Handle}";

        public static bool operator ==(TimerToken lhs, TimerToken rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                return ReferenceEquals(rhs, null);
            }
            else if (ReferenceEquals(rhs, null))
            {
                return false;
            }
            return lhs.Handle == rhs.Handle;
        }
        
        public static bool operator !=(TimerToken lhs, TimerToken rhs) => !(lhs == rhs);
    }
}