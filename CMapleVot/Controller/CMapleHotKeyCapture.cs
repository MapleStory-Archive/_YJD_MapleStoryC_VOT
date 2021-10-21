// ===============================
// @AUTHOR      : 윤정도
// @TIME        : 21-10-21
// @DESC        : 핫키 캡쳐용 라이브러리 교체
// ===============================


namespace CMapleVot
{
    // ===============================
    // @AUTHOR      : 윤정도
    // @CREATE DATE : 2020-04-04 오후 4:20:15   
    // @PURPOSE     : 글로벌 핫키 캡쳐
    // ===============================


    using Gma.System.MouseKeyHook;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public class CMapleHotkeyCapture
    {
        private Thread _hotkeySynchronizer;
        private object _hotkeySynchronizerLocker = new object();
        private IKeyboardMouseEvents _globalHook;

        public CMapleHotkeyCapture()
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += Capture_KeyDown;
            _hotkeySynchronizer = new Thread(() =>
            {
            });
            _hotkeySynchronizer.IsBackground = true;
            _hotkeySynchronizer.Start();
        }

        private void Capture_KeyDown(object sender, KeyEventArgs e)
        {
            lock (_hotkeySynchronizerLocker)
            {
                CMapleMainWindow.window.OnHotKeyPressed(CMapleExtension.WinformsToWPFKey(e.KeyCode));
            }
        }

        public void Terminate()
        {
            _globalHook.KeyDown -= Capture_KeyDown;
            _globalHook.Dispose();
        }
    }

}
