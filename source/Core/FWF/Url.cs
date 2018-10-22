using System;

namespace FWF
{
    [Serializable]
    public class Url
    {

        private readonly Uri _url;
        private const string DefaultUriString = "http://tempuri.org:0/";

        public static Url Empty;

        static Url()
        {
            Empty = new Url();
        }

        public Url()
            : this(DefaultUriString)
        {
        }

        public Url(Uri uri)
            : this(uri.ToString())
        {
        }

        public Url(string urlString)
        {
            _url = new Uri(urlString);
        }

        #region Object Overrides

        public override int GetHashCode()
        {
            return _url.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Url;

            if (other != null)
            {
                var diff = string.Compare(
                    _url.ToString(),
                    other._url.ToString(),
                    StringComparison.OrdinalIgnoreCase
                    );

                return diff == 0;
            }

            return false;
        }

        public override string ToString()
        {
            return ReferenceEquals(_url, null) ? string.Empty : _url.ToString();
        }

        #endregion

        public static implicit operator string (Url url)
        {
            if (ReferenceEquals(url, null))
            {
                return string.Empty;
            }

            return url.ToString();
        }

        public static Url FromRelativePath(string relativePath)
        {
            if (relativePath.IsMissing())
            {
                return Url.Empty;
            }

            var newPath = relativePath;

            if (newPath.StartsWith("/"))
            {
                newPath = newPath.Substring(1);
            }

            var fullPath = string.Concat(DefaultUriString, newPath);

            return new Url(fullPath);
        }

    }
}

