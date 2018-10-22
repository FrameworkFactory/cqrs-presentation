using System;

namespace FWF
{
    [Serializable]
    public class AppVersion : IComparable, IComparable<AppVersion>, IEquatable<AppVersion>
    {
        public const string InvalidAppVersionExceptionMessage = "Invalid value for AppVersion";

        private readonly int _major;
        private readonly int _minor;
        private readonly int _build;
        private readonly int _revision;

        private const int MaxMajor = 65535;
        private const int MaxMinor = 65535;
        private const int MaxBuild = 65535;
        private const int MaxRevision = 65535;

        public static AppVersion Empty = new AppVersion();
        

        #region Constructors / Finalizer

        public AppVersion(int major, int minor, int build, int revision)
        {
            if (major < 0)
                throw new ArgumentOutOfRangeException("major");

            if (minor < 0)
                throw new ArgumentOutOfRangeException("minor");

            if (build < 0)
                throw new ArgumentOutOfRangeException("build");

            if (revision < 0)
                throw new ArgumentOutOfRangeException("revision");

            _major = major;
            _minor = minor;
            _build = build;
            _revision = revision;
        }

        public AppVersion(int major, int minor, int build)
        {
            if (major < 0)
                throw new ArgumentOutOfRangeException("major");

            if (minor < 0)
                throw new ArgumentOutOfRangeException("minor");

            if (build < 0)
                throw new ArgumentOutOfRangeException("build");

            _major = major;
            _minor = minor;
            _build = build;
        }

        public AppVersion(int major, int minor)
        {
            if (major < 0)
                throw new ArgumentOutOfRangeException("major");

            if (minor < 0)
                throw new ArgumentOutOfRangeException("minor");

            _major = major;
            _minor = minor;
        }

        public AppVersion(String version)
        {
            var data = ParseNumbers(version);
            _major = data[0];
            _minor = data[1];
            _build = data[2];
            _revision = data[3];
        }

        public AppVersion()
        {
            _major = 0;
            _minor = 0;
        }

        #endregion

        #region Properties

        public int Major
        {
            get { return _major; }
        }

        public int Minor
        {
            get { return _minor; }
        }

        public int Build
        {
            get { return _build; }
        }

        public int Revision
        {
            get { return _revision; }
        }

        #endregion

        #region Operators

        public static bool operator ==(AppVersion v1, AppVersion v2)
        {
            if (Object.ReferenceEquals(v1, null))
            {
                return Object.ReferenceEquals(v2, null);
            }

            return v1.Equals(v2);
        }

        public static bool operator !=(AppVersion v1, AppVersion v2)
        {
            return !(v1 == v2);
        }

        public static bool operator <(AppVersion v1, AppVersion v2)
        {
            if ((Object)v1 == null)
                throw new ArgumentNullException("v1");

            return (v1.CompareTo(v2) < 0);
        }

        public static bool operator <=(AppVersion v1, AppVersion v2)
        {
            if ((Object)v1 == null)
                throw new ArgumentNullException("v1");

            return (v1.CompareTo(v2) <= 0);
        }

        public static bool operator >(AppVersion v1, AppVersion v2)
        {
            return (v2 < v1);
        }

        public static bool operator >=(AppVersion v1, AppVersion v2)
        {
            return (v2 <= v1);
        }

        public static implicit operator AppVersion(string versionString)
        {
            return new AppVersion(versionString);
        }

        public static implicit operator String(AppVersion appVersion)
        {
            return appVersion.ToString();
        }

        #endregion

        #region IComparable Members

        public int CompareTo(Object version)
        {
            if (version == null)
            {
                return 1;
            }

            var v = version as AppVersion;
            if (v == null)
            {
                throw new ArgumentException("Object must be of type AppVersion");
            }

            if (this._major != v._major)
                if (this._major > v._major)
                    return 1;
                else
                    return -1;

            if (this._minor != v._minor)
                if (this._minor > v._minor)
                    return 1;
                else
                    return -1;

            if (this._build != v._build)
                if (this._build > v._build)
                    return 1;
                else
                    return -1;

            if (this._revision != v._revision)
                if (this._revision > v._revision)
                    return 1;
                else
                    return -1;

            return 0;
        }

        public int CompareTo(AppVersion value)
        {
            if (value == null)
                return 1;

            if (this._major != value._major)
                if (this._major > value._major)
                    return 1;
                else
                    return -1;

            if (this._minor != value._minor)
                if (this._minor > value._minor)
                    return 1;
                else
                    return -1;

            if (this._build != value._build)
                if (this._build > value._build)
                    return 1;
                else
                    return -1;

            if (this._revision != value._revision)
                if (this._revision > value._revision)
                    return 1;
                else
                    return -1;

            return 0;
        }

        #endregion

        #region IEquatable Members

        public override bool Equals(Object obj)
        {
            AppVersion v = obj as AppVersion;
            if (v == null)
                return false;

            // check that major, minor, build & revision numbers match
            if ((this._major != v._major) ||
                (this._minor != v._minor) ||
                (this._build != v._build) ||
                (this._revision != v._revision))
                return false;

            return true;
        }

        public bool Equals(AppVersion obj)
        {
            if (obj == null)
                return false;

            // check that major, minor, build & revision numbers match
            if ((this._major != obj._major) ||
                (this._minor != obj._minor) ||
                (this._build != obj._build) ||
                (this._revision != obj._revision))
                return false;

            return true;
        }

        #endregion

        public override int GetHashCode()
        {
            // Let's assume that most version numbers will be pretty small and just
            // OR some lower order bits together.

            int accumulator = 0;

            accumulator |= (this._major & 0x0000000F) << 28;
            accumulator |= (this._minor & 0x000000FF) << 20;
            accumulator |= (this._build & 0x000000FF) << 12;
            accumulator |= (this._revision & 0x00000FFF);

            return accumulator;
        }

        public override String ToString()
        {
            return string.Format(
                "{0}.{1}.{2}.{3}",
                _major,
                _minor,
                _build,
                _revision
                );
        }

        public static AppVersion Parse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            return new AppVersion(input);
        }

        private static int[] ParseNumbers(string input)
        {
            var data = default(int[]);
            if (!TryParseNumbers(input, out data))
            {
                throw new FormatException(InvalidAppVersionExceptionMessage);
            }

            return data;
        }

        private static bool TryParseNumbers(string input, out int[] data)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            data = new int[4];
            var versionNumbers = input.Split(new char[] { '.', ',', ':' });
            if (versionNumbers.Length > 0)
            {
                if (!int.TryParse(versionNumbers[0], out data[0]))
                {
                    return false;
                }
            }
            if (versionNumbers.Length > 1)
            {
                if (!int.TryParse(versionNumbers[1], out data[1]))
                {
                    return false;
                }
            }
            if (versionNumbers.Length > 2)
            {
                if (!int.TryParse(versionNumbers[2], out data[2]))
                {
                    return false;
                }
            }
            if (versionNumbers.Length > 3)
            {
                if (!int.TryParse(versionNumbers[3], out data[3]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool TryParse(string input, out AppVersion result)
        {
            var data = default(int[]);
            if (TryParseNumbers(input, out data))
            {
                result = new AppVersion(data[0], data[1], data[2], data[3]);
                return true;
            }

            result = null;
            return false;
        }

        public byte[] BinarySerialize()
        {
            var appVersionData = new byte[8];

            appVersionData[0] = (byte)(_major);
            appVersionData[1] = (byte)(_major >> 8);
            appVersionData[2] = (byte)(_minor);
            appVersionData[3] = (byte)(_minor >> 8);
            appVersionData[4] = (byte)(_build);
            appVersionData[5] = (byte)(_build >> 8);
            appVersionData[6] = (byte)(_revision);
            appVersionData[7] = (byte)(_revision >> 8);

            return appVersionData;
        }

        public AppVersion BinaryDeserialize(byte[] versionData)
        {
            if (ReferenceEquals(versionData, null))
            {
                throw new ArgumentNullException("versionData");
            }
            if (versionData.Length < 8)
            {
                throw new ArgumentOutOfRangeException("invalid version data - must have a length of 16");
            }

            int major = versionData[0] | versionData[1] << 8;
            int minor = versionData[2] | versionData[3] << 8;
            int build = versionData[4] | versionData[5] << 8;
            int revision = versionData[6] | versionData[7] << 8;

            return new AppVersion(major, minor, build, revision);
        }
    }
}

