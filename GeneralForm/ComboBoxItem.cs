
namespace GeneralForm
{
    public class ComboBoxItem
    {
        string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
        
        public ComboBoxItem(string text, string value)
        {
            this.Text = text;
            this.Value = value;
        }
        public override string ToString()
        {
            return this.Text;
        }
    }
}
