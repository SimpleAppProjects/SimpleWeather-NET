using System;
using System.Drawing;

namespace SimpleWeather.Utils
{
    // Based on System.Version, Windows..PackageVersion
    public struct Version : ICloneable, IComparable, IComparable<Version>, IEquatable<Version>
    {
        public int Major;
        public int Minor;
        public int Build;

        public Version(int major, int minor, int build)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(major);
            ArgumentOutOfRangeException.ThrowIfNegative(minor);
            ArgumentOutOfRangeException.ThrowIfNegative(build);

            Major = major;
            Minor = minor;
            Build = build;
        }

        public Version(string version)
        {
            var v = Parse(version);
            Major = v.Major;
            Minor = v.Minor;
            Build = v.Build;
        }

        public static readonly Version Default;

        private Version(Version version)
        {
            Major = version.Major;
            Minor = version.Minor;
            Build = version.Build;
        }

        public readonly object Clone()
        {
            return new Version(this);
        }

        public static bool operator ==(Version v1, Version v2)
        {
            if (v1.Major == v2.Major && v1.Minor == v2.Minor)
            {
                return v1.Build == v2.Build;
            }

            return false;
        }

        public static bool operator !=(Version v1, Version v2)
        {
            return !(v1 == v2);
        }

        public static bool operator <(Version v1, Version v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator <=(Version v1, Version v2)
        {
            return v1.CompareTo(v2) <= 0;
        }

        public static bool operator >(Version v1, Version v2) => v2 < v1;

        public static bool operator >=(Version v1, Version v2) => v2 <= v1;

        public readonly int CompareTo(object version)
        {
            if (version == null)
            {
                return 1;
            }

            if (version is Version v)
            {
                return CompareTo(v);
            }

            throw new ArgumentException("Not instance of version", nameof(version));
        }

        public readonly int CompareTo(Version value)
        {
            return
                Equals(value, this) ? 0 :
                Major != value.Major ? (Major > value.Major ? 1 : -1) :
                Minor != value.Minor ? (Minor > value.Minor ? 1 : -1) :
                Build != value.Build ? (Build > value.Build ? 1 : -1) :
                0;
        }

        public readonly bool Equals(Version other)
        {
            return this == other;
        }

        public readonly override bool Equals(object obj)
        {
            if (obj is Version packageVersion)
            {
                return this == packageVersion;
            }

            return false;
        }

        public readonly override int GetHashCode()
        {
            return Major.GetHashCode() ^ Minor.GetHashCode() ^ Build.GetHashCode();
        }

        public readonly override string ToString()
        {
            return $"{Major}.{Minor}.{Build}";
        }

        public static Version Parse(string versionString)
        {
            var parts = versionString?.Split('.') ?? throw new ArgumentNullException(nameof(versionString));

            return parts.Length switch
            {
                3 => new Version(major: int.Parse(parts[0]), minor: int.Parse(parts[1]), build: int.Parse(parts[2])),
                2 => new Version(major: int.Parse(parts[0]), minor: int.Parse(parts[1]), build: 0),
                1 => new Version(major: int.Parse(parts[0]), minor: 0, build: 0),
                _ => throw new ArgumentException("Invalid version string", nameof(versionString)),
            };
        }

        public static Version Create(int major, int minor = 0, int build = 0)
        {
            return new Version(major, minor, build);
        }

        public readonly bool IsAtLeast(int major, int minor = 0, int build = 0)
        {
            if (this.Major != major)
                return this.Major > major;
            if (this.Minor != minor)
                return this.Minor > minor;

            return this.Build >= build;
        }

        public readonly bool IsNotAtLeast(int major, int minor = 0, int build = 0)
        {
            return !IsAtLeast(major, minor, build);
        }
    }
}
