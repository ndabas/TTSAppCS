//
// TTSAppCS
// 
// TTSAppCS is a C# clone of the C++ and Visual Basic
// TTSApp/TTSAppVB samples in the Microsoft Speech SDK 5.1.
// 
// Nikhil Dabas, ndabas@hotmail.com
// 

namespace NikhilDabas.TTSApp
{
	using System;
	using System.Data;
	using System.IO;
	using System.Windows.Forms;
	using System.Drawing;
	using SpeechLib;
	
	[Serializable]
	public class TTSAppForm : Form
	{
		
		private Button CloseButton;
		private Button ResetButton;
		private Button AboutButton;
		private Button SaveWavButton;
		private NumericUpDown SkipUpDown;
		private Button SkipButton;
		private Button StopButton;
		private Button PauseResumeButton;
		private Button SpeakButton;
		private Button FileOpenButton;
		private GroupBox SpeechFlagsFrame;
		private CheckBox SpeakPunctuationCheck;
		private CheckBox PersistXmlCheck;
		private CheckBox IsNotXmlCheck;
		private CheckBox IsXmlCheck;
		private CheckBox IsFilenameCheck;
		private CheckBox ShowAllEventsCheck;
		private TextBox EventsTextBox;
		private ComboBox OutputCombo;
		private ComboBox FormatCombo;
		private TrackBar VolumeSlider;
		private TrackBar RateSlider;
		private ComboBox VoiceCombo;
		private RichTextBox InputTextBox;
		private PictureBox MouthPicture;
		private Label OutputLabel;
		private Label FormatLabel;
		private Label VolumeLabel;
		private Label RateLabel;
		private Label VoiceLabel;
		private System.ComponentModel.Container components = null;
		
		private SpVoiceClass Voice;
		private ImageList MicImages;
		private bool IsPaused = false;
		
		public TTSAppForm()
		{
			InitializeComponent();
			
			System.Resources.ResourceManager resources =
				new System.Resources.ResourceManager(typeof(TTSAppForm));
			
			try
			{
				Voice = new SpVoiceClass();
			}
			catch(Exception)
			{
				// This might fail if the user does not have
				// Speech Automation 5.1 Runtimes installed.
				MessageBox.Show("An error has occurred while trying to initialize SAPI 5.1.");
				Application.Exit();
			}
			
			// We're interested in all events, so hook them up.
			Voice.EventInterests = SpeechVoiceEvents.SVEAllEvents;
			Voice.AudioLevel += new  _ISpeechVoiceEvents_AudioLevelEventHandler(this.Voice_AudioLevel);
			Voice.Bookmark += new  _ISpeechVoiceEvents_BookmarkEventHandler(this.Voice_Bookmark);
			Voice.EndStream += new  _ISpeechVoiceEvents_EndStreamEventHandler(this.Voice_EndStream);
			Voice.EnginePrivate += new  _ISpeechVoiceEvents_EnginePrivateEventHandler(this.Voice_EnginePrivate);
			Voice.Phoneme += new  _ISpeechVoiceEvents_PhonemeEventHandler(this.Voice_Phoneme);
			Voice.Sentence += new  _ISpeechVoiceEvents_SentenceEventHandler(this.Voice_Sentence);
			Voice.StartStream += new  _ISpeechVoiceEvents_StartStreamEventHandler(this.Voice_StartStream);
			Voice.Viseme += new  _ISpeechVoiceEvents_VisemeEventHandler(this.Voice_Viseme);
			Voice.VoiceChange += new  _ISpeechVoiceEvents_VoiceChangeEventHandler(this.Voice_VoiceChange);
			Voice.Word += new  _ISpeechVoiceEvents_WordEventHandler(this.Voice_Word);
			
			// Populate the list of voices.
			ISpeechObjectTokens voices = Voice.GetVoices("", "");
			foreach(ISpeechObjectToken token in voices)
			{
				VoiceCombo.Items.Add(token.GetDescription(0));
			}
			
			// Populate the list of output devices.
			ISpeechObjectTokens outputs = Voice.GetAudioOutputs("", "");
			foreach(ISpeechObjectToken token in outputs)
			{
				OutputCombo.Items.Add(token.GetDescription(0));
			}
			
			// This is better than what has been done in the
			// Microsoft samples.
			string[] formats = Enum.GetNames(typeof(SpeechAudioFormatType));
			for(int i = 0; i < formats.Length; i++)
			{
				FormatCombo.Items.Add(formats[i]);
			}
			
			// Load the talking microphone images. I have swiped
			// these directly from the Microsoft CPP sample.
			this.MicImages = new ImageList();
			this.MicImages.TransparentColor = Color.Fuchsia;
			this.MicImages.ColorDepth = ColorDepth.Depth24Bit;
			this.MicImages.ImageSize = new Size(128, 128);
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_mouth_2.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_mouth_3.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_mouth_4.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_mouth_5.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_mouth_6.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_mouth_7.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_mouth_8.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_mouth_9.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_mouth_10.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_mouth_11.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_mouth_12.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_mouth_13.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_eyes_closed.bmp"));
			this.MicImages.Images.Add((Bitmap)resources.GetObject("mic_eyes_narrow.bmp"));
			
			this.MouthPicture.Image = MicImages.Images[0];
			
			this.VolumeSlider.Value = Voice.Volume;
			this.RateSlider.Value = Voice.Rate;
			
			InputTextBox.ReadOnly = false;
			SkipButton.Enabled = false;
			SpeakButton.Enabled = true;
			PauseResumeButton.Enabled = false;
			StopButton.Enabled = false;
			
			this.InputTextBox.Focus();
		}
		
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		
		public void InitializeComponent()
		{
			this.CloseButton = new Button();
			this.ResetButton = new Button();
			this.AboutButton = new Button();
			this.SaveWavButton = new Button();
			this.SkipUpDown = new NumericUpDown();
			this.SkipButton = new Button();
			this.StopButton = new Button();
			this.PauseResumeButton = new Button();
			this.SpeakButton = new Button();
			this.FileOpenButton = new Button();
			this.SpeechFlagsFrame = new GroupBox();
			this.IsFilenameCheck = new CheckBox();
			this.IsXmlCheck = new CheckBox();
			this.IsNotXmlCheck = new CheckBox();
			this.PersistXmlCheck = new CheckBox();
			this.SpeakPunctuationCheck = new CheckBox();
			this.ShowAllEventsCheck = new CheckBox();
			this.EventsTextBox = new TextBox();
			this.OutputCombo = new ComboBox();
			this.FormatCombo = new ComboBox();
			this.VolumeSlider = new TrackBar();
			this.RateSlider = new TrackBar();
			this.VoiceCombo = new ComboBox();
			this.InputTextBox = new RichTextBox();
			this.MouthPicture = new PictureBox();
			this.OutputLabel = new Label();
			this.FormatLabel = new Label();
			this.VolumeLabel = new Label();
			this.RateLabel = new Label();
			this.VoiceLabel = new Label();
			this.SpeechFlagsFrame.SuspendLayout();
			this.SuspendLayout();
			
			this.Text = "TTS Demo";
			this.Size = new Size(563, 474);
			this.MinimumSize = new Size(563, 474);
			this.Font = new Font("Tahoma", 8);
			this.AcceptButton = SpeakButton;
			this.CancelButton = CloseButton;
			
			this.CloseButton.Text = "Close";
			this.CloseButton.Size = new Size(105, 25);
			this.CloseButton.Location = new Point(440, 408);
			this.CloseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.CloseButton.FlatStyle = FlatStyle.System;
			this.CloseButton.Click += new EventHandler(this.CloseButton_Click);
			
			this.ResetButton.Text = "Reset";
			this.ResetButton.Size = new Size(105, 25);
			this.ResetButton.Location = new Point(440, 200);
			this.ResetButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.ResetButton.FlatStyle = FlatStyle.System;
			this.ResetButton.Click += new EventHandler(this.ResetButton_Click);
			
			this.AboutButton.Text = "About";
			this.AboutButton.Size = new Size(105, 25);
			this.AboutButton.Location = new Point(440, 376);
			this.AboutButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.AboutButton.FlatStyle = FlatStyle.System;
			this.AboutButton.Click += new EventHandler(this.AboutButton_Click);
			
			this.SaveWavButton.Text = "Save to .wav file...";
			this.SaveWavButton.Size = new Size(105, 25);
			this.SaveWavButton.Location = new Point(440, 168);
			this.SaveWavButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.SaveWavButton.FlatStyle = FlatStyle.System;
			this.SaveWavButton.Click += new EventHandler(this.SaveWavButton_Click);
			
			this.SkipUpDown.Text = "0";
			this.SkipUpDown.Size = new Size(43, 25);
			this.SkipUpDown.Location = new Point(496, 138);
			this.SkipUpDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			
			this.SkipButton.Text = "Skip";
			this.SkipButton.Size = new Size(49, 25);
			this.SkipButton.Location = new Point(440, 136);
			this.SkipButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.SkipButton.FlatStyle = FlatStyle.System;
			this.SkipButton.Click += new EventHandler(this.SkipButton_Click);
			
			this.StopButton.Text = "Stop";
			this.StopButton.Size = new Size(105, 25);
			this.StopButton.Location = new Point(440, 104);
			this.StopButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.StopButton.FlatStyle = FlatStyle.System;
			this.StopButton.Click += new EventHandler(this.StopButton_Click);
			
			this.PauseResumeButton.Text = "Pause";
			this.PauseResumeButton.Size = new Size(105, 25);
			this.PauseResumeButton.Location = new Point(440, 72);
			this.PauseResumeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.PauseResumeButton.FlatStyle = FlatStyle.System;
			this.PauseResumeButton.Click += new EventHandler(this.PauseResumeButton_Click);
			
			this.SpeakButton.Text = "Speak";
			this.SpeakButton.Size = new Size(105, 25);
			this.SpeakButton.Location = new Point(440, 40);
			this.SpeakButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.SpeakButton.FlatStyle = FlatStyle.System;
			this.SpeakButton.Click += new EventHandler(this.SpeakButton_Click);
			
			this.FileOpenButton.Text = "Open File...";
			this.FileOpenButton.Size = new Size(105, 25);
			this.FileOpenButton.Location = new Point(440, 8);
			this.FileOpenButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.FileOpenButton.FlatStyle = FlatStyle.System;
			this.FileOpenButton.Click += new EventHandler(this.FileOpenButton_Click);
			
			this.SpeechFlagsFrame.Text = "Speech Flags";
			this.SpeechFlagsFrame.Size = new Size(121, 153);
			this.SpeechFlagsFrame.FlatStyle = FlatStyle.System;
			this.SpeechFlagsFrame.Location = new Point(8, 168);
			
			this.IsFilenameCheck.Text = "Is Filename";
			this.IsFilenameCheck.Size = new Size(105, 17);
			this.IsFilenameCheck.FlatStyle = FlatStyle.System;
			this.IsFilenameCheck.Location = new Point(8, 16);
			
			this.IsXmlCheck.Text = "Is XML";
			this.IsXmlCheck.Size = new Size(105, 17);
			this.IsXmlCheck.Location = new Point(8, 40);
			this.IsXmlCheck.FlatStyle = FlatStyle.System;
			
			this.IsNotXmlCheck.Text = "Is Not XML";
			this.IsNotXmlCheck.Size = new Size(105, 17);
			this.IsNotXmlCheck.Location = new Point(8, 64);
			this.IsNotXmlCheck.FlatStyle = FlatStyle.System;
			
			this.PersistXmlCheck.Text = "Persist XML";
			this.PersistXmlCheck.Size = new Size(105, 17);
			this.PersistXmlCheck.Location = new Point(8, 88);
			this.PersistXmlCheck.FlatStyle = FlatStyle.System;
			
			this.SpeakPunctuationCheck.Text = "Speak Punctuation";
			this.SpeakPunctuationCheck.Size = new Size(97, 33);
			this.SpeakPunctuationCheck.Location = new Point(8, 112);
			this.SpeakPunctuationCheck.FlatStyle = FlatStyle.System;
			
			this.ShowAllEventsCheck.Text = "Show all events";
			this.ShowAllEventsCheck.Size = new Size(121, 17);
			this.ShowAllEventsCheck.Location = new Point(8, 328);
			this.ShowAllEventsCheck.FlatStyle = FlatStyle.System;
			this.ShowAllEventsCheck.Anchor = AnchorStyles.Bottom |
				AnchorStyles.Left;
			
			this.EventsTextBox.Size = new Size(425, 81);
			this.EventsTextBox.Location = new Point(8, 352);
			this.EventsTextBox.Multiline = true;
			this.EventsTextBox.ReadOnly = true;
			this.EventsTextBox.ScrollBars = ScrollBars.Vertical;
			this.EventsTextBox.Anchor = AnchorStyles.Bottom |
				AnchorStyles.Left | AnchorStyles.Right;
			
			this.OutputCombo.Width = 233;
			this.OutputCombo.Location = new Point(200, 288);
			this.OutputCombo.DropDownStyle = ComboBoxStyle.DropDownList;
			this.OutputCombo.Anchor = AnchorStyles.Bottom | AnchorStyles.Right |
				AnchorStyles.Left;
			
			this.FormatCombo.Width = 233;
			this.FormatCombo.Location = new Point(200, 320);
			this.FormatCombo.DropDownStyle = ComboBoxStyle.DropDownList;
			this.FormatCombo.Anchor = AnchorStyles.Bottom | AnchorStyles.Right |
				AnchorStyles.Left;
			
			this.VolumeSlider.Size = new Size(233, 25);
			this.VolumeSlider.Location = new Point(200, 248);
			this.VolumeSlider.Anchor = AnchorStyles.Bottom | AnchorStyles.Right |
				AnchorStyles.Left;
			this.VolumeSlider.Minimum = 0;
			this.VolumeSlider.Maximum = 100;
			this.VolumeSlider.TickFrequency = 10;
			this.VolumeSlider.ValueChanged += new EventHandler(this.VolumeSlider_ValueChanged);
			
			this.RateSlider.Size = new Size(233, 25);
			this.RateSlider.Location = new Point(200, 216);
			this.RateSlider.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;
			this.RateSlider.Minimum = -10;
			this.RateSlider.Maximum = 10;
			this.RateSlider.TickFrequency = 2;
			this.RateSlider.ValueChanged += new EventHandler(this.RateSlider_ValueChanged);
			
			this.VoiceCombo.Width = 233;
			this.VoiceCombo.Location = new Point(200, 192);
			this.VoiceCombo.Anchor = AnchorStyles.Bottom | AnchorStyles.Right |
				AnchorStyles.Left;
			this.VoiceCombo.DropDownStyle = ComboBoxStyle.DropDownList;
			this.VoiceCombo.SelectedValueChanged += new EventHandler(this.VoiceCombo_SelectedValueChanged);
			
			this.InputTextBox.Text = "Enter text to be spoken here.";
			this.InputTextBox.Size = new Size(297, 170);
			this.InputTextBox.Location = new Point(136, 8);
			this.InputTextBox.Multiline = true;
			this.InputTextBox.HideSelection = false;
			this.InputTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
				AnchorStyles.Left | AnchorStyles.Right;
			
			this.MouthPicture.Size = new Size(129, 153);
			this.MouthPicture.ClientSize = new Size(121, 153);
			this.MouthPicture.Location = new Point(8, 8);
			this.MouthPicture.SizeMode = PictureBoxSizeMode.CenterImage;
			
			this.OutputLabel.Text = "Device:";
			this.OutputLabel.Size = new Size(57, 17);
			this.OutputLabel.Location = new Point(136, 288);
			this.OutputLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			this.OutputLabel.FlatStyle = FlatStyle.System;
			
			this.FormatLabel.Text = "Format:";
			this.FormatLabel.Size = new Size(57, 17);
			this.FormatLabel.Location = new Point(136, 320);
			this.FormatLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			this.FormatLabel.FlatStyle = FlatStyle.System;
			
			this.VolumeLabel.Text = "Volume:";
			this.VolumeLabel.Size = new Size(57, 17);
			this.VolumeLabel.Location = new Point(136, 256);
			this.VolumeLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			this.VolumeLabel.FlatStyle = FlatStyle.System;
			
			this.RateLabel.Text = "Rate:";
			this.RateLabel.Size = new Size(57, 17);
			this.RateLabel.Location = new Point(136, 224);
			this.RateLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			this.RateLabel.FlatStyle = FlatStyle.System;
			
			this.VoiceLabel.Text = "Voice:";
			this.VoiceLabel.Size = new Size(57, 17);
			this.VoiceLabel.Location = new Point(136, 192);
			this.VoiceLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			this.VoiceLabel.FlatStyle = FlatStyle.System;
			
			this.SpeechFlagsFrame.Controls.AddRange(new System.Windows.Forms.Control[]
			{
				this.SpeakPunctuationCheck,
				this.PersistXmlCheck,
				this.IsNotXmlCheck,
				this.IsXmlCheck,
				this.IsFilenameCheck
			});
			
			this.Controls.AddRange(new System.Windows.Forms.Control[]
			{
				this.CloseButton,
				this.ResetButton,
				this.AboutButton,
				this.SaveWavButton,
				this.SkipUpDown,
				this.SkipButton,
				this.StopButton,
				this.PauseResumeButton,
				this.SpeakButton,
				this.FileOpenButton,
				this.SpeechFlagsFrame,
				this.ShowAllEventsCheck,
				this.EventsTextBox,
				this.OutputCombo,
				this.FormatCombo,
				this.VolumeSlider,
				this.RateSlider,
				this.VoiceCombo,
				this.InputTextBox,
				this.MouthPicture,
				this.OutputLabel,
				this.FormatLabel,
				this.VolumeLabel,
				this.RateLabel,
				this.VoiceLabel
			});
			
			this.ResumeLayout(false);
			SpeechFlagsFrame.ResumeLayout(false);
		}
		
		[STAThread]
		static void Main(string[] args)
		{
			Application.Run(new TTSAppForm());
		}
		
		// Writes a message to the list of events.
		private void AddEventMessage(string message)
		{
			EventsTextBox.AppendText(message + "\r\n");
		}
		
		private void Speak()
		{
			SpeechVoiceSpeakFlags flags =
				SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak | SpeechVoiceSpeakFlags.SVSFlagsAsync;
			flags |= SpeakPunctuationCheck.Checked ? SpeechVoiceSpeakFlags.SVSFNLPSpeakPunc : 0;
			flags |= PersistXmlCheck.Checked ? SpeechVoiceSpeakFlags.SVSFPersistXML : 0;
			flags |= IsNotXmlCheck.Checked ? SpeechVoiceSpeakFlags.SVSFIsNotXML : 0;
			flags |= IsXmlCheck.Checked ? SpeechVoiceSpeakFlags.SVSFIsXML : 0;
			flags |= IsFilenameCheck.Checked ? SpeechVoiceSpeakFlags.SVSFIsFilename : 0;
			try
			{
				Voice.Speak(InputTextBox.Text, flags);
			}
			catch(Exception)
			{
				AddEventMessage("ERROR: Error while trying to speak.");
			}
		}
		
		private void CloseButton_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}
		
		private void ResetButton_Click(object sender, System.EventArgs e)
		{
			Voice.Speak(null, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
			if(IsPaused)
			{
				PauseResumeButton.Text = "Pause";
				Voice.Resume();
				IsPaused = false;
			}
			
			MouthPicture.Image = MicImages.Images[0];
			
			SpeakPunctuationCheck.Checked = false;
			PersistXmlCheck.Checked = false;
			IsNotXmlCheck.Checked = false;
			IsXmlCheck.Checked = false;
			IsFilenameCheck.Checked = false;
			ShowAllEventsCheck.Checked = false;
			
			EventsTextBox.Clear();
			InputTextBox.Clear();
			SkipUpDown.Text = "0";
			
			Voice.Volume = VolumeSlider.Value;
			Voice.Rate = RateSlider.Value;
			
			AddEventMessage("INFO: Reset");
		}
		
		private void AboutButton_Click(object sender, System.EventArgs e)
		{
			string message = "TTSAppCS 1.0\n\n";
			message += "Created by Nikhil Dabas, based on the ";
			message += "TTSApp tool sample in Microsoft Speech SDK 5.1.";
			MessageBox.Show(message);
		}
		
		private void SaveWavButton_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog saveFile = new SaveFileDialog();
			saveFile.Filter = "All Files (*.*)|*.*|Wave Files (*.wav)|*.wav";
			saveFile.FilterIndex = 2;
			saveFile.RestoreDirectory = true;
			if(saveFile.ShowDialog() == DialogResult.OK)
			{
				AddEventMessage("INFO: Saving to a wave file...");
				
				// We need a file stream.
				SpFileStreamClass spFile = new SpFileStreamClass();
				
				// Try and use the user specified format for the
				// wave file.
				if(FormatCombo.SelectedIndex != -1)
				{
					try
					{
						SpeechAudioFormatType newFormat = (SpeechAudioFormatType)
							Enum.Parse(typeof(SpeechAudioFormatType), (string)FormatCombo.SelectedItem);
						spFile.Format.Type = newFormat;
					}
					catch(Exception)
					{
						AddEventMessage("ERROR: Error while attempting to set audio format.");
					}
				}
				
				spFile.Open(saveFile.FileName, SpeechStreamFileMode.SSFMCreateForWrite, false);
				
				// We don't want SAPI to change the format, because
				// we might have done so already.
				Voice.AllowAudioOutputFormatChangesOnNextSet = false;
				Voice.AudioOutputStream = spFile;
				this.Speak();
				Voice.WaitUntilDone(-1);
				
				spFile.Close();
				
				AddEventMessage("INFO: Wave file written successfully.");
			}
		}
		
		private void SkipButton_Click(object sender, System.EventArgs e)
		{
			Int32 num = Int32.Parse(SkipUpDown.Text);
			
			// Currently, the only available dimension is Sentence.
			Voice.Skip("Sentence", num);
		}
		
		private void StopButton_Click(object sender, System.EventArgs e)
		{
			Voice.Speak(null, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
			if(IsPaused)
			{
				PauseResumeButton.Text = "Pause";
				Voice.Resume();
				IsPaused = false;
			}
			
			MouthPicture.Image = MicImages.Images[0];
		}
		
		private void PauseResumeButton_Click(object sender, System.EventArgs e)
		{
			if(IsPaused)
			{
				PauseResumeButton.Text = "Pause";
				Voice.Resume();
				IsPaused = false;
			}
			else
			{
				PauseResumeButton.Text = "Resume";
				Voice.Pause();
				IsPaused = true;
			}
		}
		
		private void SpeakButton_Click(object sender, System.EventArgs e)
		{
			int index;
			
			// Try to use the user selected device.
			if(OutputCombo.SelectedIndex != -1)
			{
				try
				{
					Voice.AllowAudioOutputFormatChangesOnNextSet = true;
					Voice.AudioOutput = Voice.GetAudioOutputs("", "").Item(OutputCombo.SelectedIndex);
				}
				catch(Exception)
				{
					AddEventMessage("ERROR: Error while attempting to set output device.");
				}
			}
			
			// Update the combobox to reflect the device SAPI is
			// actually using.
			index = OutputCombo.FindStringExact(Voice.AudioOutput.GetDescription(0));
			if(index != -1)
			{
				OutputCombo.SelectedIndex = index;
			}
			
			Type formatType = typeof(SpeechAudioFormatType);
			
			// Try to use the user selected format.
			if(FormatCombo.SelectedIndex != -1)
			{
				try
				{
					SpeechAudioFormatType newFormat = (SpeechAudioFormatType)
						Enum.Parse(formatType, (string)FormatCombo.SelectedItem);
					Voice.AllowAudioOutputFormatChangesOnNextSet = false;
					Voice.AudioOutputStream.Format.Type = newFormat;
					Voice.AudioOutputStream = Voice.AudioOutputStream;
				}
				catch(Exception)
				{
					AddEventMessage("ERROR: Error while attempting to set audio format.");
				}
			}
			
			// Update the combobox to reflect the format
			// actually being used.
			string formatName = Enum.GetName(formatType, Voice.AudioOutputStream.Format.Type);
			index = FormatCombo.FindStringExact(formatName);
			if(index != -1)
			{
				FormatCombo.SelectedIndex = index;
			}
			
			this.Speak();
		}
		
		private void FileOpenButton_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog openFile = new OpenFileDialog();
			
			openFile.Filter = "Text files (*.txt)|*.txt|XML files (*.xml)|*.xml|All files (*.*)|*.*" ;
			openFile.FilterIndex = 2;
			openFile.RestoreDirectory = true;
			
			if(openFile.ShowDialog() == DialogResult.OK)
			{
				InputTextBox.LoadFile(openFile.FileName, RichTextBoxStreamType.PlainText);
			}
		}
		
		private void VolumeSlider_ValueChanged(object sender, System.EventArgs e)
		{
			Voice.Volume = VolumeSlider.Value;
		}
		
		private void RateSlider_ValueChanged(object sender, System.EventArgs e)
		{
			Voice.Rate = RateSlider.Value;
		}
		
		private void VoiceCombo_SelectedValueChanged(object sender, System.EventArgs e)
		{
			Voice.Voice = Voice.GetVoices("", "").Item(VoiceCombo.SelectedIndex);
		}
		
		private void Voice_AudioLevel(int StreamNumber, object StreamPosition, int AudioLevel)
		{
			if(ShowAllEventsCheck.Checked)
			{
				AddEventMessage("AudioLevel: AudioLevel = " + AudioLevel.ToString());
			}
		}
		
		private void Voice_Bookmark(int StreamNumber, object StreamPosition, string Bookmark, int BookmarkId)
		{
			AddEventMessage("Bookmark: Bookmark = " + Bookmark + ", BookmarkId = " + BookmarkId.ToString());
		}
		
		private void Voice_EndStream(int StreamNumber, object StreamPosition)
		{
			AddEventMessage("EndStream");
			
			InputTextBox.ReadOnly = false;
			SkipButton.Enabled = false;
			SpeakButton.Enabled = true;
			PauseResumeButton.Enabled = false;
			StopButton.Enabled = false;
			
			SpeakPunctuationCheck.Enabled = true;
			PersistXmlCheck.Enabled = true;
			IsNotXmlCheck.Enabled = true;
			IsXmlCheck.Enabled = true;
			IsFilenameCheck.Enabled = true;
		}
		
		private void Voice_EnginePrivate(int StreamNumber, int StreamPosition, object EngineData)
		{
			if(ShowAllEventsCheck.Checked)
			{
				AddEventMessage("EnginePrivate: EngineData = " + EngineData.ToString());
			}
		}
		
		private void Voice_Phoneme(int StreamNumber, object StreamPosition, int Duration, short NextPhoneId, SpeechVisemeFeature Feature, short CurrentPhoneId)
		{
			if(ShowAllEventsCheck.Checked)
			{
				AddEventMessage("Phoneme: Duration = " + Duration.ToString() +
					", NextPhoneId = " + NextPhoneId.ToString() +
					", CurrentPhoneId = " + CurrentPhoneId.ToString() +
					", Feature = " + Feature.ToString());
			}
		}
		
		private void Voice_Sentence(int StreamNumber, object StreamPosition, int CharacterPosition, int Length)
		{
			if(ShowAllEventsCheck.Checked)
			{
				AddEventMessage("Sentence: CharacterPosition = " + CharacterPosition.ToString() + ", Length = " + Length.ToString());
			}
		}
		
		private void Voice_StartStream(int StreamNumber, object StreamPosition)
		{
			AddEventMessage("StartStream");
			
			InputTextBox.ReadOnly = true;
			SkipButton.Enabled = true;
			SpeakButton.Enabled = false;
			PauseResumeButton.Enabled = true;
			StopButton.Enabled = true;
			
			SpeakPunctuationCheck.Enabled = false;
			PersistXmlCheck.Enabled = false;
			IsNotXmlCheck.Enabled = false;
			IsXmlCheck.Enabled = false;
			IsFilenameCheck.Enabled = false;
		}
		
		private void Voice_Viseme(int StreamNumber, object StreamPosition, int Duration, SpeechVisemeType NextVisemeId, SpeechVisemeFeature Feature, SpeechVisemeType CurrentVisemeId)
		{
			if(ShowAllEventsCheck.Checked)
			{
				AddEventMessage("Viseme: Duration = " + Duration.ToString() +
					", NextVisemeId = " + NextVisemeId.ToString() +
					", Feature = " + Feature.ToString() +
					", CurrentVisemeId = " + CurrentVisemeId.ToString());
			}
			
			// Show some mouth animation, though this is not
			// really accurate.
			int[] visemeMap = {0, 11, 11, 11, 10, 11, 9, 2, 13, 9, 12, 11, 9, 3, 6, 7, 8, 5, 4, 7, 9, 1};
			int index;
			Graphics g = Graphics.FromImage(MouthPicture.Image);
			g.Clear(Color.Transparent);
			g.DrawImage(MicImages.Images[0], new Point(0, 0));
			index = visemeMap[Convert.ToInt32(CurrentVisemeId)];
			g.DrawImage(MicImages.Images[index], new Point(0, 0));
			if(index % 6 == 2)
			{
				g.DrawImage(MicImages.Images[14], new Point(0, 0));
			}
			if(index % 6 == 5)
			{
				g.DrawImage(MicImages.Images[13], new Point(0, 0));
			}
			MouthPicture.Invalidate();
		}
		
		private void Voice_VoiceChange(int StreamNumber, object StreamPosition, SpObjectToken VoiceObjectToken)
		{
			int index = VoiceCombo.FindStringExact(VoiceObjectToken.GetDescription(0));
			if(index != -1)
			{
				VoiceCombo.SelectedIndex = index;
			}
			
			AddEventMessage("VoiceChange: Voice = " + VoiceObjectToken.GetDescription(0));
		}
		
		private void Voice_Word(int StreamNumber, object StreamPosition, int CharacterPosition, int Length)
		{
			// Don't track in a filename.
			if(!IsFilenameCheck.Checked)
			{
				InputTextBox.Select(CharacterPosition, Length);
			}
			
			if(ShowAllEventsCheck.Checked)
			{
				AddEventMessage("Word:  CharacterPosition = " + CharacterPosition.ToString() +
					", Length = " + Length.ToString());
			}
		}
	}
}