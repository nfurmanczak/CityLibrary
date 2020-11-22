using System;

namespace CityLibrary.Ui
{
    public class UiMenuItem
    {
        public UiMenuItem(char key, string info, Action action) {
            this.Key = key;
            this.Info = info;
            this.Action = action;
        }

        public char Key;
        public string Info;
        public Action Action;
    }
}
