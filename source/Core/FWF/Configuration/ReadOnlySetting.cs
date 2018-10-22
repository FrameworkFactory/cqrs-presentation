
namespace FWF.Configuration
{
    public class ReadOnlySetting
    {

        private readonly string _name;
        private readonly string _default;

        public ReadOnlySetting(string name, string @default)
        {
            _name = name;
            _default = @default;
        }

        public string Name
        {
            get { return _name; }
        }

        public string @Default
        {
            get { return _default; }
        }

        
    }
}

