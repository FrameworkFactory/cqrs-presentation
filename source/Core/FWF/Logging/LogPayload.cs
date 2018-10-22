using System;
using System.Collections.Generic;

namespace FWF.Logging
{
    public class LogPayload
    {
        
        public DateTime Timestamp { get; set; }

        public string Name { get; set; }

        public LogLevel LogLevel { get; set; }

        public string EnvironmentName { get; set; }

        public Guid? ClientId { get; set; }

        public string ClientVersion { get; set; }
        
        public Guid? ClientInstanceId { get; set; }

        public string MachineName { get; set; }

        public Guid? TenantId { get; set; }

        public Guid? UserId { get; set; }

        public string UserName { get; set; }

        public Guid? SessionId { get; set; }

        public Guid? DeviceId { get; set; }

        public Guid? CorrelationId { get; set; }

        public string Source { get; set; }

        public string Message { get; set; }

        public string Callstack { get; set; }

        public IDictionary<string, string> Properties { get; set; }
        
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return ReferenceEquals(this, obj) || (GetHashCode() == obj.GetHashCode());
        }

        public static bool operator ==(LogPayload left, LogPayload right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LogPayload left, LogPayload right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            unchecked // overflow is fine, just wrap
            {
                var hash = 17;
                hash *= 23 + LogLevel.GetHashCode();
                hash *= 23 + (EnvironmentName != null ? EnvironmentName.GetHashCode() : 0);
                hash *= 23 + (ClientId.HasValue ? ClientId.GetHashCode() : 0);
                hash *= 23 + (ClientVersion != null ? ClientVersion.GetHashCode() : 0);
                hash *= 23 + (MachineName != null ? MachineName.GetHashCode() : 0);
                hash *= 23 + (TenantId.HasValue ? TenantId.GetHashCode() : 0);
                hash *= 23 + (UserId.HasValue ? UserId.GetHashCode() : 0);
                hash *= 23 + (UserName != null ? UserName.GetHashCode() : 0);
                hash *= 23 + (SessionId.HasValue ? SessionId.GetHashCode() : 0);
                hash *= 23 + (DeviceId.HasValue ? DeviceId.GetHashCode() : 0);
                hash *= 23 + (CorrelationId.HasValue ? CorrelationId.GetHashCode() : 0);
                hash *= 23 + (Source != null ? Source.GetHashCode() : 0);
                hash *= 23 + (Message != null ? Message.GetHashCode() : 0);
                hash *= 23 + (Callstack != null ? Callstack.GetHashCode() : 0);
                return hash;
            }
        }
    }
}


