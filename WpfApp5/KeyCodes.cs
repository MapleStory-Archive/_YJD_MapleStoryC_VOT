using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5
{
    public enum WMessages : int
    {
        WM_LBUTTONDOWN = 0x201, //Left mousebutton down
        WM_LBUTTONUP = 0x202,  //Left mousebutton up
        WM_LBUTTONDBLCLK = 0x203, //Left mousebutton doubleclick
        WM_RBUTTONDOWN = 0x204, //Right mousebutton down
        WM_RBUTTONUP = 0x205,   //Right mousebutton up
        WM_RBUTTONDBLCLK = 0x206, //Right mousebutton doubleclick
        WM_KEYDOWN = 0x100,  //Key down
        WM_KEYUP = 0x101,   //Key up
    }

    public enum VKeys : int
    {
        None = 0x00,
        //LeftMouseButton = 0x01,   //Left mouse button 
        //RightMouseButton = 0x02,   //Right mouse button 
        //Cancel = 0x03,   //Control-break processing 
        //MiddleMouseButton = 0x04,   //Middle mouse button (three-button mouse) 
        //Backspace = 0x08,   //BACKSPACE key 
        //Tab = 0x09,   //TAB key 
        //Clear = 0x0C,   //CLEAR key 
        //Enter = 0x0D,   //ENTER key 
        Shift = 0x10,   //SHIFT key 
        Ctrl = 0x11,   //CTRL key 
        Alt = 0x12,   //ALT key 
        Puase = 0x13,   //PAUSE key 
        CapsLock = 0x14,   //CAPS LOCK key 
        Esc = 0x1B,   //ESC key 
        Space = 0x20,   //SPACEBAR 
        PageUp = 0x21,   //PAGE UP key 
        PageDown = 0x22,   //PAGE DOWN key 
        End = 0x23,   //END key 
        Home = 0x24,   //HOME key 
        Insert = 0x2D,   //INS key 
        Delete = 0x2E,   //DEL key 
        ArrowLeft = 0x25,   //LEFT ARROW key 
        ArrowUp = 0x26,   //UP ARROW key 
        ArrowRight = 0x27,   //RIGHT ARROW key 
        ArrowDown = 0x28,   //DOWN ARROW key 
        _0 = 0x30,   //0 key 
        _1 = 0x31,   //1 key 
        _2 = 0x32,   //2 key 
        _3 = 0x33,   //3 key 
        _4 = 0x34,   //4 key 
        _5 = 0x35,   //5 key 
        _6 = 0x36,    //6 key 
        _7 = 0x37,    //7 key 
        _8 = 0x38,   //8 key 
        _9 = 0x39,    //9 key 
        A = 0x41,   //A key 
        B = 0x42,   //B key 
        C = 0x43,   //C key 
        D = 0x44,   //D key 
        E = 0x45,   //E key 
        F = 0x46,   //F key 
        G = 0x47,   //G key 
        H = 0x48,   //H key 
        I = 0x49,    //I key 
        J = 0x4A,   //J key 
        K = 0x4B,   //K key 
        L = 0x4C,   //L key 
        M = 0x4D,   //M key 
        N = 0x4E,    //N key 
        O = 0x4F,   //O key 
        P = 0x50,    //P key 
        Q = 0x51,   //Q key 
        R = 0x52,   //R key 
        S = 0x53,   //S key 
        T = 0x54,   //T key 
        U = 0x55,   //U key 
        V = 0x56,   //V key 
        W = 0x57,   //W key 
        X = 0x58,   //X key 
        Y = 0x59,   //Y key 
        Z = 0x5A,    //Z key
        //VK_NUMPAD0 = 0x60,   //Numeric keypad 0 key 
        //VK_NUMPAD1 = 0x61,   //Numeric keypad 1 key 
        //VK_NUMPAD2 = 0x62,   //Numeric keypad 2 key 
        //VK_NUMPAD3 = 0x63,   //Numeric keypad 3 key 
        //VK_NUMPAD4 = 0x64,   //Numeric keypad 4 key 
        //VK_NUMPAD5 = 0x65,   //Numeric keypad 5 key 
        //VK_NUMPAD6 = 0x66,   //Numeric keypad 6 key 
        //VK_NUMPAD7 = 0x67,   //Numeric keypad 7 key 
        //VK_NUMPAD8 = 0x68,   //Numeric keypad 8 key 
        //VK_NUMPAD9 = 0x69,   //Numeric keypad 9 key 
        //VK_SEPARATOR = 0x6C,   //Separator key 
        //VK_SUBTRACT = 0x6D,   //Subtract key 
        //VK_DECIMAL = 0x6E,   //Decimal key 
        //VK_DIVIDE = 0x6F,   //Divide key
        F1 = 0x70,   //F1 key 
        F2 = 0x71,   //F2 key 
        F3 = 0x72,   //F3 key 
        F4 = 0x73,   //F4 key 
        F5 = 0x74,   //F5 key 
        F6 = 0x75,   //F6 key 
        F7 = 0x76,   //F7 key 
        F8 = 0x77,   //F8 key 
        F9 = 0x78,   //F9 key 
        F10 = 0x79,   //F10 key 
        F11 = 0x7A,   //F11 key 
        F12 = 0x7B,   //F12 key
        //VK_SCROLL = 0x91,   //SCROLL LOCK key 
        //VK_LSHIFT = 0xA0,   //Left SHIFT key
        //VK_RSHIFT = 0xA1,   //Right SHIFT key
        //VK_LCONTROL = 0xA2,   //Left CONTROL key
        //VK_RCONTROL = 0xA3,    //Right CONTROL key
        //VK_LMENU = 0xA4,      //Left MENU key
        //VK_RMENU = 0xA5,   //Right MENU key
        //VK_PLAY = 0xFA,   //Play key
        //VK_ZOOM = 0xFB, //Zoom key 
    }
}
