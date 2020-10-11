﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrKlang;
using System.Windows.Forms;
using System.Drawing;

namespace SoundBoardDotNet
{
    public class SoundButtonMaker
    {
        public SelectSound SoundForm;
        public bool HasSound => Data.FilePath != "";
        public Button Btn;
        public SoundButtonData Data;

        public string Name;

        public static Form Parent;
        public static int Scale = 10;
        public static Point Origin;

        public static char KeyToChar(Keys k)
        {
            
            switch (k)
            {
                case Keys.D0:
                    return (char)48;
                case Keys.D1:
                    return (char)49;
                case Keys.D2:
                    return '2';
                case Keys.D3:
                    return '3';
                case Keys.D4:
                    return '4';
                case Keys.D5:
                    return '5';
                case Keys.D6:
                    return '6';
                case Keys.D7:
                    return '7';
                case Keys.D8:
                    return '8';
                case Keys.D9:
                    return '9';
                case Keys.A:
                    return 'A';
                case Keys.B:
                    return 'B';
                case Keys.C:
                    return 'C';
                case Keys.D:
                    return 'D';
                case Keys.E:
                    return 'E';
                case Keys.F:
                    return 'F';
                case Keys.G:
                    return 'G';
                case Keys.H:
                    return 'H';
                case Keys.I:
                    return 'I';
                case Keys.J:
                    return 'J';
                case Keys.K:
                    return 'K';
                case Keys.L:
                    return 'L';
                case Keys.M:
                    return 'M';
                case Keys.N:
                    return 'N';
                case Keys.O:
                    return 'O';
                case Keys.P:
                    return 'P';
                case Keys.Q:
                    return 'Q';
                case Keys.R:
                    return 'R';
                case Keys.S:
                    return 'S';
                case Keys.T:
                    return 'T';
                case Keys.U:
                    return 'U';
                case Keys.V:
                    return 'V';
                case Keys.W:
                    return 'W';
                case Keys.X:
                    return 'X';
                case Keys.Y:
                    return 'Y';
                case Keys.Z:
                    return 'Z';
                case Keys.OemMinus:
                    return '-';
                case Keys.Oem3:
                    return '`';
                case Keys.Oem7:
                    if (SoundBoardProperties.Props.Keyboard == KeyboardType.QWERTY_FR)
                        return '#';
                    return '\'';
                case Keys.Oemplus:
                    return '=';
                case Keys.Oem4:
                    return '[';
                case Keys.Oem6:
                    return ']';
                case Keys.Oem5:
                    return '\\';
                case Keys.Oemcomma:
                    return ',';
                case Keys.OemPeriod:
                    return '.';
                case Keys.Space:
                    return ' ';
                case Keys.Oem1:
                    return ';';
                case Keys.Oem2:
                    return '/';
                default:
                    return (char)0;
            }
        }

        public SoundButtonMaker(int x, int y, int height, int width, string name)
        {
            Name = name;
            Data = new SoundButtonData();
            SoundForm = new SelectSound(UpdateBtnText, Data);
            Btn = new Button();
            Btn.Click += Btn_Click;
            Btn.Size = new Size(width * Scale, height * Scale);
            Btn.Location = new Point(Origin.X + x * Scale, Origin.Y + y * Scale);
            Btn.Text = name;
            Btn.TextAlign = ContentAlignment.TopCenter;
            Parent.Controls.Add(Btn);
        }

        public void Update()
        {
            SoundForm = new SelectSound(UpdateBtnText, Data);
            Btn.Text = Name;
            if (SoundForm.Sound != null)
            {
                if (SoundForm.Sound.WaveStream != null) Btn.Text += "\n" + Data.Name;
            }
        }

        private void UpdateBtnText()
        {
            Btn.Text = $"{Name}\n{SoundForm.GetNameTextBox.Text}";
        }

        virtual protected void Btn_Click(object sender, EventArgs e)
        {
            if (SoundForm == null) SoundForm = new SelectSound(UpdateBtnText, Data);
            if (!SoundForm.IsDisposed)
            {
                SoundForm.LoadFromData();
                SoundForm.Show();
            }
            else
            {
                SoundForm = new SelectSound(UpdateBtnText, Data);
                SoundForm.Show();
            }
            Form1.SetFocus();
        }
    }
}
