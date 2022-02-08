using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Shapes;

namespace PianoKeyboard.Extensions
{
    public class KeyLayout
    {
        private MainWindow window;
        private int keyLayoutId;

        private int transpose = 0;

        public struct KeyCode
        {
            public int rawNoteNum;
            public Key jp;
            public Key en_us;
        }

        public List<KeyCode> keyCodes = new List<KeyCode>{
            new KeyCode{ rawNoteNum = 36, jp = Key.Q, en_us = Key.Q },
            new KeyCode{ rawNoteNum = 37, jp = Key.W, en_us = Key.W },
            new KeyCode{ rawNoteNum = 38, jp = Key.E, en_us = Key.E },
            new KeyCode{ rawNoteNum = 39, jp = Key.R, en_us = Key.R },
            new KeyCode{ rawNoteNum = 40, jp = Key.T, en_us = Key.T },
            new KeyCode{ rawNoteNum = 41, jp = Key.Y, en_us = Key.Y },
            new KeyCode{ rawNoteNum = 42, jp = Key.U, en_us = Key.U },
            new KeyCode{ rawNoteNum = 43, jp = Key.I, en_us = Key.I },
            new KeyCode{ rawNoteNum = 44, jp = Key.O, en_us = Key.O },
            new KeyCode{ rawNoteNum = 45, jp = Key.P, en_us = Key.P },
            new KeyCode{ rawNoteNum = 46, jp = Key.A, en_us = Key.A },
            new KeyCode{ rawNoteNum = 47, jp = Key.S, en_us = Key.S },
            new KeyCode{ rawNoteNum = 48, jp = Key.D, en_us = Key.D },
            new KeyCode{ rawNoteNum = 49, jp = Key.F, en_us = Key.F },
            new KeyCode{ rawNoteNum = 50, jp = Key.G, en_us = Key.G },
            new KeyCode{ rawNoteNum = 51, jp = Key.H, en_us = Key.H },
            new KeyCode{ rawNoteNum = 52, jp = Key.J, en_us = Key.J },
            new KeyCode{ rawNoteNum = 53, jp = Key.K, en_us = Key.K },
            new KeyCode{ rawNoteNum = 54, jp = Key.L, en_us = Key.L },
            new KeyCode{ rawNoteNum = 55, jp = Key.Z, en_us = Key.Z },
            new KeyCode{ rawNoteNum = 56, jp = Key.X, en_us = Key.X },
            new KeyCode{ rawNoteNum = 57, jp = Key.C, en_us = Key.C },
            new KeyCode{ rawNoteNum = 58, jp = Key.V, en_us = Key.V },
            new KeyCode{ rawNoteNum = 59, jp = Key.B, en_us = Key.B },
            new KeyCode{ rawNoteNum = 60, jp = Key.N, en_us = Key.N },
            new KeyCode{ rawNoteNum = 61, jp = Key.M, en_us = Key.M },
            new KeyCode{ rawNoteNum = 62, jp = Key.F1, en_us = Key.F1 },
            new KeyCode{ rawNoteNum = 63, jp = Key.F2, en_us = Key.F2 },
            new KeyCode{ rawNoteNum = 64, jp = Key.F3, en_us = Key.F3 },
            new KeyCode{ rawNoteNum = 65, jp = Key.F4, en_us = Key.F4 },
            new KeyCode{ rawNoteNum = 66, jp = Key.F5, en_us = Key.F5 },
            new KeyCode{ rawNoteNum = 67, jp = Key.F6, en_us = Key.F6 },
            new KeyCode{ rawNoteNum = 68, jp = Key.F7, en_us = Key.F7 },
            new KeyCode{ rawNoteNum = 69, jp = Key.F8, en_us = Key.F8 },
            new KeyCode{ rawNoteNum = 70, jp = Key.F9, en_us = Key.F9 },
            new KeyCode{ rawNoteNum = 71, jp = Key.F10, en_us = Key.F10 },
            new KeyCode{ rawNoteNum = 72, jp = Key.F11, en_us = Key.F11 },
        };

        private List<NoteConverter.NoteMap> activeKeyList = new List<NoteConverter.NoteMap>();

        public KeyLayout(MainWindow _window)
        {
            window = _window;
            keyLayoutId = CultureInfo.CurrentCulture.KeyboardLayoutId;
        }

        public void KeyDownHandler(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if ((transpose + 12) > 60) break;
                    resetKeyList();
                    transpose += 12;
                    window.TransposeItam.Content = $"Transpose: {transpose}";
                    break;

                case Key.Down:
                    if ((transpose - 12) < -48) break;
                    resetKeyList();
                    transpose -= 12;
                    window.TransposeItam.Content = $"Transpose: {transpose}";
                    break;

                case Key.Right:
                    if ((transpose + 1) > 60) break;
                    resetKeyList();
                    transpose += 1;
                    window.TransposeItam.Content = $"Transpose: {transpose}";
                    break;

                case Key.Left:
                    if ((transpose - 1) < -48) break;
                    resetKeyList();
                    transpose -= 1;
                    window.TransposeItam.Content = $"Transpose: {transpose}";
                    break;
            }

            if (!keyCodes.Any(n => Equals(((keyLayoutId == 1041) ? n.jp : n.en_us), e.Key))) return;
            KeyCode key = keyCodes.Find(n => Equals((keyLayoutId == 1041) ? n.jp : n.en_us, e.Key));
            int noteNum = key.rawNoteNum + transpose;
            if (noteNum < 0 || noteNum > 127) return;
            NoteConverter.NoteMap note = window.noteConverter.GetNote(noteNum);
            if (activeKeyList.Any(x => Equals(x.noteName, note.noteName))) return;
            window.NoteOnHandler(0x90, note);
            activeKeyList.Add(note);
        }

        public void KeyUpHandler(object sender, KeyEventArgs e)
        {
            if (!keyCodes.Any(n => Equals(((keyLayoutId == 1041) ? n.jp : n.en_us), e.Key))) return;
            KeyCode key = keyCodes.Find(n => Equals((keyLayoutId == 1041) ? n.jp : n.en_us, e.Key));
            int noteNum = key.rawNoteNum + transpose;
            if (noteNum < 0 || noteNum > 127) return;
            NoteConverter.NoteMap note = window.noteConverter.GetNote(noteNum);
            if (activeKeyList.Any(x => Equals(x.noteName, note.noteName)))
            {
                activeKeyList.Remove(note);
            }
            window.NoteOffHandler(0x80, note);
        }

        private void resetKeyList()
        {
            for (int key = 0; key < activeKeyList.Count; key++)
            {
                NoteConverter.NoteMap activeKey = activeKeyList[key];
                window.NoteOffHandler(0x80, activeKey);
            }
        }
    }
}