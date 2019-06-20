﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IrrKlang;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using NAudio;
using NAudio.Wave;
using System.IO;

namespace SoundBoardDotNet
{
    public partial class SelectSound : Form
    {
        public AudioSound Sound;
        public string FileName;

        public TextBox GetNameTextBox { get { return NameTextBox; } }
        public TextBox GetFileTextBox { get { return FileNameBox; } }
        //public WaveForm MyWaveForm { get { return WaveGraph; } }

        private OpenFileDialog _fileDialog = new OpenFileDialog();
        public SoundButtonData Data;

        public delegate void CallBack();

        private CallBack _cb;

        public SelectSound(CallBack cb, SoundButtonData data)
        {
            Data = data;
            _cb = cb;
            _fileDialog.Filter = "All playable files (*.wav,*.mp3)|*.wav;*.WAV;*.mp3;*.MP3";
            _fileDialog.FilterIndex = 0;
            InitializeComponent();
            AddActionsForControlsOfTypes((Control c) => c.KeyDown += PlayStopOnKeys, typeof(Button), typeof(ComboBox), typeof(NumericUpDown));
            AddActionsForControlsOfTypes((Control c) => c.KeyDown += SelectNextOnEnterKey, typeof(ComboBox), typeof(NumericUpDown), typeof(TextBox));
            AddActionsForControlsOfTypes((Control c) => c.KeyDown += SpaceForNumUpDown, typeof(NumericUpDown));
            AddArrowSelectForControls(StartTime, EndTime, SaveButton, CancelButton);
            AddActionsForControlsOfTypes((Control c) => { c.KeyDown += CloseOnEsc; c.KeyDown += SupressKeys; }, typeof(Button), typeof(ComboBox), typeof(NumericUpDown), typeof(TextBox));

            LoadFromData();
            //Sound = new AudioSloaound(data.FilePath, data.Slider1, data.Slider2, data.Volume);
        }

        private void SupressKeys(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Tab))
            {
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        private void AddActionsForControlsOfTypes(Action<Control> action, params Type[] types)
        {
            foreach (var control in Controls)
            {
                foreach (var type in types)
                {
                    if (control.GetType() == type)
                    {
                        action((Control)control);
                    }
                }
            }
        }

        private void AddArrowSelectForControls(params Control[] controls)
        {
            foreach (var control in controls)
            {
                control.KeyDown += SelectWithArrows;
            }
        }

        private void SpaceForNumUpDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                e.Handled = e.SuppressKeyPress = true;
                ((NumericUpDown)sender).Refresh();
            }
        }

        private void SelectNextOnEnterKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SelectNextControl(ActiveControl, true, false, false, true);
                return;
            }
            e.Handled = false;
            e.SuppressKeyPress = false;
        }

        private void PlayStopOnKeys(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (e.Control)
                {
                    Stop();
                }
                else
                {
                    Play();
                }
                return;
            }
            e.Handled = true;
            e.SuppressKeyPress = false;
        }

        private void CloseOnEsc(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
                return;
            }

            e.SuppressKeyPress = false;
        }

        private void SelectWithArrows(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SelectNextControl(ActiveControl, true, false, false, true);
                return;
            }
            if (e.KeyCode == Keys.Left)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SelectNextControl(ActiveControl, false, false, false, true);
                return;
            }
            e.SuppressKeyPress = false;
        }

        private byte[] _soundWaves(string file)
        {
            //Engine.RemoveSoundSource(file);
            //var source = Engine.AddSoundSourceFromFile(file);
            //if (source != null)
            //{
            //    return source.SampleData;
            //}

            //get data
            return new byte[0];
        }

        private void _updateGraph(bool eraseIfEmpty = false)
        {
            if (FileNameBox.Text == "")
            {
                if (eraseIfEmpty)
                {
                    TotalTimeLabel.Text = "";
                    EndTime.Maximum = 0;
                    WaveGraph.WaveStream = null;
                }
                return;
            }
            if (Sound != null)
            {
                if (Sound.FileReader == null)
                {
                    if (eraseIfEmpty)
                    {
                        TotalTimeLabel.Text = "";
                        EndTime.Maximum = 0;
                        WaveGraph.WaveStream = null;
                    }
                    return;
                }
            }

            Sound = new AudioSound(FileNameBox.Text, (double)StartTime.Value, (double)EndTime.Value, VolumeControl.Volume);

            WaveGraph.WaveStream = Sound.FileReader;
            var temp = EndTime.Value;
            EndTime.Maximum = (decimal)Sound.FileReader.TotalTime.TotalSeconds;
            if (temp == 0)
                EndTime.Value = EndTime.Maximum;
            TotalTimeLabel.Text = $"{EndTime.Maximum} s";
            VolumeControl.Volume = 1;
        }

        private void _updateData()
        {
            if (FileNameBox.Text == "") return;
            FileInfo file = new FileInfo(FileNameBox.Text);

            if (!file.Exists) throw new FileNotFoundException("File not found!", file.FullName);
            Form1.MyForm.HasChanged = true;
            Data.FilePath = FileNameBox.Text;
            Data.Name = NameTextBox.Text;
            Data.Volume = VolumeControl.Volume;
            Data.StartTime = (double)StartTime.Value;
            Data.EndTime = (double)EndTime.Value;
        }

        private uint _percentToTime(double percent, uint time)
        {
            var output = Convert.ToUInt32(Math.Round(percent * time / 100, 0));
            return output;
        }

        public void PlaySoundAsync()
        {
            //var sound = AudioSound.AddSound(FileNameBox.Text, WaveGraph.Slider1Value, WaveGraph.Slider2Value, (float)VolumeTrack.Value / 100);
            //return sound != null;// true if successfull
            if (Sound == null) return;
            AudioSound.PlaySound(Sound);
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            _fileDialog.ShowDialog();
            FileNameBox.Text = _fileDialog.FileName;
            var pathList = FileNameBox.Text.Split('\\');
            var extensionList = pathList[pathList.Length - 1].Split('.');
            NameTextBox.Text = extensionList[0];
            _updateGraph();
        }

        private void FileNameBox_TextChanged(object sender, EventArgs e)
        {
            var pathList = FileNameBox.Text.Split('\\');
            var extensionList = pathList[pathList.Length - 1].Split('.');
            NameTextBox.Text = extensionList[0];
            _updateGraph();
        }

        private void Play()
        {
            if (FileNameBox.Text == "")
            {
                MessageBox.Show("Enter a valid File to play!");
                return;
            }
            AudioSound.PlaySound(Sound);
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            Play();
        }

        private void Stop()
        {
            AudioSound.StopAll();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void SelectSound_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Stop sounds
            //if (Sound != null) Sound.Stop();
            AudioSound.StopAll();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                _updateData();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Invalid file name!");
                return;
            }

            _cb();
            Hide();
        }

        public void LoadFromData()
        {
            if (Data.FilePath != "")
            {
                if (Sound == null) Sound = new AudioSound(Data.FilePath, Data.StartTime, Data.EndTime, Data.Volume);
                else
                {
                    if (Sound.FileName != Data.FilePath) Sound = new AudioSound(Data.FilePath, Data.StartTime, Data.EndTime, Data.Volume);
                }
                if (Sound.FileReader == null) return;
                Data.FilePath = Sound.FileName;
                FileNameBox.Text = Data.FilePath;
                WaveGraph.WaveStream = Sound.FileReader;
                var temp = EndTime.Maximum;
                EndTime.Maximum = (decimal)Sound.FileReader.TotalTime.TotalSeconds;
                if (temp == 0)
                    EndTime.Value = EndTime.Maximum;
                TotalTimeLabel.Text = $"{EndTime.Maximum} s";
                VolumeControl.Volume = 1;
            }

            NameTextBox.Text = Data.Name;
            StartTime.Value = (decimal)Data.StartTime;
            EndTime.Value = (decimal)Data.EndTime;
            VolumeControl.Volume = Data.Volume;
        }

        private void SelectSound_Shown(object sender, EventArgs e)
        {
            LoadFromData();
        }

        private void SelectSound_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            LoadFromData();
            _updateGraph(true);

            e.Cancel = true;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Hide();
            LoadFromData();
            _updateGraph(true);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            FileNameBox.Text = "";
            NameTextBox.Text = "";
            Data.Reset();
            _updateGraph(true);
        }

        private void Volume_VolumeChanged(object sender, EventArgs e)
        {
            if (Sound != null) Sound.Volume = VolumeControl.Volume;
        }

        private void StartTime_ValueChanged(object sender, EventArgs e)
        {
            EndTime.Minimum = StartTime.Value;
            Sound.StartPos = (double)StartTime.Value;
        }

        private void EndTime_ValueChanged(object sender, EventArgs e)
        {
            StartTime.Maximum = EndTime.Value;
            Sound.EndPos = (double)EndTime.Value;
        }
    }
}