using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public class MessageFilter : IMessageFilter
    {
        IMessageFilterReceiver receiver;
        public MessageFilter(IMessageFilterReceiver receiver)
        {
            this.receiver = receiver;
        }
        public bool PreFilterMessage(ref Message message)
        {
            return receiver.OnMessageFilterMessageReceived(ref message);
        }
    }
}


//Keys keyCode = (Keys)(int)m.WParam & Keys.KeyCode;
//if (m.Msg == WM_KEYDOWN && keyCode == Keys.Escape)
//{
//}
//Keys keyCode = (Keys)(int)message.WParam & Keys.KeyCode;
//if (keyCode == Keys.Down)
//{
//}

//{ 0     , _T("WM_NULL")}
//  , { 1     , _T("WM_CREATE")}
//  , { 2     , _T("WM_DESTROY")}
//  , { 3     , _T("WM_MOVE")}
//  , { 5     , _T("WM_SIZE")}
//  , { 6     , _T("WM_ACTIVATE")}
//  , { 7     , _T("WM_SETFOCUS")}
//  , { 8     , _T("WM_KILLFOCUS")}
//  , { 10    , _T("WM_ENABLE")}
//  , { 11    , _T("WM_SETREDRAW")}
//  , { 12    , _T("WM_SETTEXT")}
//  , { 13    , _T("WM_GETTEXT")}
//  , { 14    , _T("WM_GETTEXTLENGTH")}
//  , { 15    , _T("WM_PAINT")}
//  , { 16    , _T("WM_CLOSE")}
//  , { 17    , _T("WM_QUERYENDSESSION")}
//  , { 19    , _T("WM_QUERYOPEN")}
//  , { 22    , _T("WM_ENDSESSION")}
//  , { 18    , _T("WM_QUIT")}
//  , { 20    , _T("WM_ERASEBKGND")}
//  , { 21    , _T("WM_SYSCOLORCHANGE")}
//  , { 24    , _T("WM_SHOWWINDOW")}
//  , { 26    , _T("WM_WININICHANGE")}
//  , { 27    , _T("WM_DEVMODECHANGE")}
//  , { 28    , _T("WM_ACTIVATEAPP")}
//  , { 29    , _T("WM_FONTCHANGE")}
//  , { 30    , _T("WM_TIMECHANGE")}
//  , { 31    , _T("WM_CANCELMODE")}
//  , { 32    , _T("WM_SETCURSOR")}
//  , { 33    , _T("WM_MOUSEACTIVATE")}
//  , { 34    , _T("WM_CHILDACTIVATE")}
//  , { 35    , _T("WM_QUEUESYNC")}
//  , { 36    , _T("WM_GETMINMAXINFO")}
//  , { 38    , _T("WM_PAINTICON")}
//  , { 39    , _T("WM_ICONERASEBKGND")}
//  , { 40    , _T("WM_NEXTDLGCTL")}
//  , { 42    , _T("WM_SPOOLERSTATUS")}
//  , { 43    , _T("WM_DRAWITEM")}
//  , { 44    , _T("WM_MEASUREITEM")}
//  , { 45    , _T("WM_DELETEITEM")}
//  , { 46    , _T("WM_VKEYTOITEM")}
//  , { 47    , _T("WM_CHARTOITEM")}
//  , { 48    , _T("WM_SETFONT")}
//  , { 49    , _T("WM_GETFONT")}
//  , { 50    , _T("WM_SETHOTKEY")}
//  , { 51    , _T("WM_GETHOTKEY")}
//  , { 55    , _T("WM_QUERYDRAGICON")}
//  , { 57    , _T("WM_COMPAREITEM")}
//  , { 61    , _T("WM_GETOBJECT")}
//  , { 65    , _T("WM_COMPACTING")}
//  , { 68    , _T("WM_COMMNOTIFY")}
//  , { 70    , _T("WM_WINDOWPOSCHANGING")}
//  , { 71    , _T("WM_WINDOWPOSCHANGED")}
//  , { 72    , _T("WM_POWER")}
//  , { 74    , _T("WM_COPYDATA")}
//  , { 75    , _T("WM_CANCELJOURNAL")}
//  , { 78    , _T("WM_NOTIFY")}
//  , { 80    , _T("WM_INPUTLANGCHANGEREQUEST")}
//  , { 81    , _T("WM_INPUTLANGCHANGE")}
//  , { 82    , _T("WM_TCARD")}
//  , { 83    , _T("WM_HELP")}
//  , { 84    , _T("WM_USERCHANGED")}
//  , { 85    , _T("WM_NOTIFYFORMAT")}
//  , { 123   , _T("WM_CONTEXTMENU")}
//  , { 124   , _T("WM_STYLECHANGING")}
//  , { 125   , _T("WM_STYLECHANGED")}
//  , { 126   , _T("WM_DISPLAYCHANGE")}
//  , { 127   , _T("WM_GETICON")}
//  , { 128   , _T("WM_SETICON")}
//  , { 129   , _T("WM_NCCREATE")}
//  , { 130   , _T("WM_NCDESTROY")}
//  , { 131   , _T("WM_NCCALCSIZE")}
//  , { 132   , _T("WM_NCHITTEST")}
//  , { 133   , _T("WM_NCPAINT")}
//  , { 134   , _T("WM_NCACTIVATE")}
//  , { 135   , _T("WM_GETDLGCODE")}
//  , { 136   , _T("WM_SYNCPAINT")}
//  , { 160   , _T("WM_NCMOUSEMOVE")}
//  , { 161   , _T("WM_NCLBUTTONDOWN")}
//  , { 162   , _T("WM_NCLBUTTONUP")}
//  , { 163   , _T("WM_NCLBUTTONDBLCLK")}
//  , { 164   , _T("WM_NCRBUTTONDOWN")}
//  , { 165   , _T("WM_NCRBUTTONUP")}
//  , { 166   , _T("WM_NCRBUTTONDBLCLK")}
//  , { 167   , _T("WM_NCMBUTTONDOWN")}
//  , { 168   , _T("WM_NCMBUTTONUP")}
//  , { 169   , _T("WM_NCMBUTTONDBLCLK")}
//  , { 171   , _T("WM_NCXBUTTONDOWN")}
//  , { 172   , _T("WM_NCXBUTTONUP")}
//  , { 173   , _T("WM_NCXBUTTONDBLCLK")}
//  , { 254   , _T("WM_INPUT_DEVICE_CHANGE")}
//  , { 255   , _T("WM_INPUT")}
//  , { 256   , _T("WM_KEYFIRST")}
//  , { 256   , _T("WM_KEYDOWN")}
//  , { 257   , _T("WM_KEYUP")}
//  , { 258   , _T("WM_CHAR")}
//  , { 259   , _T("WM_DEADCHAR")}
//  , { 260   , _T("WM_SYSKEYDOWN")}
//  , { 261   , _T("WM_SYSKEYUP")}
//  , { 262   , _T("WM_SYSCHAR")}
//  , { 263   , _T("WM_SYSDEADCHAR")}
//  , { 265   , _T("WM_UNICHAR")}
//  , { 265   , _T("WM_KEYLAST")}
//  , { 264   , _T("WM_KEYLAST")}
//  , { 269   , _T("WM_IME_STARTCOMPOSITION")}
//  , { 270   , _T("WM_IME_ENDCOMPOSITION")}
//  , { 271   , _T("WM_IME_COMPOSITION")}
//  , { 271   , _T("WM_IME_KEYLAST")}
//  , { 272   , _T("WM_INITDIALOG")}
//  , { 273   , _T("WM_COMMAND")}
//  , { 274   , _T("WM_SYSCOMMAND")}
//  , { 275   , _T("WM_TIMER")}
//  , { 276   , _T("WM_HSCROLL")}
//  , { 277   , _T("WM_VSCROLL")}
//  , { 278   , _T("WM_INITMENU")}
//  , { 279   , _T("WM_INITMENUPOPUP")}
//  , { 281   , _T("WM_GESTURE")}
//  , { 282   , _T("WM_GESTURENOTIFY")}
//  , { 287   , _T("WM_MENUSELECT")}
//  , { 288   , _T("WM_MENUCHAR")}
//  , { 289   , _T("WM_ENTERIDLE")}
//  , { 290   , _T("WM_MENURBUTTONUP")}
//  , { 291   , _T("WM_MENUDRAG")}
//  , { 292   , _T("WM_MENUGETOBJECT")}
//  , { 293   , _T("WM_UNINITMENUPOPUP")}
//  , { 294   , _T("WM_MENUCOMMAND")}
//  , { 295   , _T("WM_CHANGEUISTATE")}
//  , { 296   , _T("WM_UPDATEUISTATE")}
//  , { 297   , _T("WM_QUERYUISTATE")}
//  , { 306   , _T("WM_CTLCOLORMSGBOX")}
//  , { 307   , _T("WM_CTLCOLOREDIT")}
//  , { 308   , _T("WM_CTLCOLORLISTBOX")}
//  , { 309   , _T("WM_CTLCOLORBTN")}
//  , { 310   , _T("WM_CTLCOLORDLG")}
//  , { 311   , _T("WM_CTLCOLORSCROLLBAR")}
//  , { 312   , _T("WM_CTLCOLORSTATIC")}
//  , { 512   , _T("WM_MOUSEFIRST")}
//  , { 512   , _T("WM_MOUSEMOVE")}
//  , { 513   , _T("WM_LBUTTONDOWN")}
//  , { 514   , _T("WM_LBUTTONUP")}
//  , { 515   , _T("WM_LBUTTONDBLCLK")}
//  , { 516   , _T("WM_RBUTTONDOWN")}
//  , { 517   , _T("WM_RBUTTONUP")}
//  , { 518   , _T("WM_RBUTTONDBLCLK")}
//  , { 519   , _T("WM_MBUTTONDOWN")}
//  , { 520   , _T("WM_MBUTTONUP")}
//  , { 521   , _T("WM_MBUTTONDBLCLK")}
//  , { 522   , _T("WM_MOUSEWHEEL")}
//  , { 523   , _T("WM_XBUTTONDOWN")}
//  , { 524   , _T("WM_XBUTTONUP")}
//  , { 525   , _T("WM_XBUTTONDBLCLK")}
//  , { 526   , _T("WM_MOUSEHWHEEL")}
//  , { 526   , _T("WM_MOUSELAST")}
//  , { 525   , _T("WM_MOUSELAST")}
//  , { 522   , _T("WM_MOUSELAST")}
//  , { 521   , _T("WM_MOUSELAST")}
//  , { 528   , _T("WM_PARENTNOTIFY")}
//  , { 529   , _T("WM_ENTERMENULOOP")}
//  , { 530   , _T("WM_EXITMENULOOP")}
//  , { 531   , _T("WM_NEXTMENU")}
//  , { 532   , _T("WM_SIZING")}
//  , { 533   , _T("WM_CAPTURECHANGED")}
//  , { 534   , _T("WM_MOVING")}
//  , { 536   , _T("WM_POWERBROADCAST")}
//  , { 537   , _T("WM_DEVICECHANGE")}
//  , { 544   , _T("WM_MDICREATE")}
//  , { 545   , _T("WM_MDIDESTROY")}
//  , { 546   , _T("WM_MDIACTIVATE")}
//  , { 547   , _T("WM_MDIRESTORE")}
//  , { 548   , _T("WM_MDINEXT")}
//  , { 549   , _T("WM_MDIMAXIMIZE")}
//  , { 550   , _T("WM_MDITILE")}
//  , { 551   , _T("WM_MDICASCADE")}
//  , { 552   , _T("WM_MDIICONARRANGE")}
//  , { 553   , _T("WM_MDIGETACTIVE")}
//  , { 560   , _T("WM_MDISETMENU")}
//  , { 561   , _T("WM_ENTERSIZEMOVE")}
//  , { 562   , _T("WM_EXITSIZEMOVE")}
//  , { 563   , _T("WM_DROPFILES")}
//  , { 564   , _T("WM_MDIREFRESHMENU")}
//  , { 576   , _T("WM_TOUCH")}
//  , { 641   , _T("WM_IME_SETCONTEXT")}
//  , { 642   , _T("WM_IME_NOTIFY")}
//  , { 643   , _T("WM_IME_CONTROL")}
//  , { 644   , _T("WM_IME_COMPOSITIONFULL")}
//  , { 645   , _T("WM_IME_SELECT")}
//  , { 646   , _T("WM_IME_CHAR")}
//  , { 648   , _T("WM_IME_REQUEST")}
//  , { 656   , _T("WM_IME_KEYDOWN")}
//  , { 657   , _T("WM_IME_KEYUP")}
//  , { 673   , _T("WM_MOUSEHOVER")}
//  , { 675   , _T("WM_MOUSELEAVE")}
//  , { 672   , _T("WM_NCMOUSEHOVER")}
//  , { 674   , _T("WM_NCMOUSELEAVE")}
//  , { 689   , _T("WM_WTSSESSION_CHANGE")}
//  , { 704   , _T("WM_TABLET_FIRST")}
//  , { 735   , _T("WM_TABLET_LAST")}
//  , { 768   , _T("WM_CUT")}
//  , { 769   , _T("WM_COPY")}
//  , { 770   , _T("WM_PASTE")}
//  , { 771   , _T("WM_CLEAR")}
//  , { 772   , _T("WM_UNDO")}
//  , { 773   , _T("WM_RENDERFORMAT")}
//  , { 774   , _T("WM_RENDERALLFORMATS")}
//  , { 775   , _T("WM_DESTROYCLIPBOARD")}
//  , { 776   , _T("WM_DRAWCLIPBOARD")}
//  , { 777   , _T("WM_PAINTCLIPBOARD")}
//  , { 778   , _T("WM_VSCROLLCLIPBOARD")}
//  , { 779   , _T("WM_SIZECLIPBOARD")}
//  , { 780   , _T("WM_ASKCBFORMATNAME")}
//  , { 781   , _T("WM_CHANGECBCHAIN")}
//  , { 782   , _T("WM_HSCROLLCLIPBOARD")}
//  , { 783   , _T("WM_QUERYNEWPALETTE")}
//  , { 784   , _T("WM_PALETTEISCHANGING")}
//  , { 785   , _T("WM_PALETTECHANGED")}
//  , { 786   , _T("WM_HOTKEY")}
//  , { 791   , _T("WM_PRINT")}
//  , { 792   , _T("WM_PRINTCLIENT")}
//  , { 793   , _T("WM_APPCOMMAND")}
//  , { 794   , _T("WM_THEMECHANGED")}
//  , { 797   , _T("WM_CLIPBOARDUPDATE")}
//  , { 798   , _T("WM_DWMCOMPOSITIONCHANGED")}
//  , { 799   , _T("WM_DWMNCRENDERINGCHANGED")}
//  , { 800   , _T("WM_DWMCOLORIZATIONCOLORCHANGED")}
//  , { 801   , _T("WM_DWMWINDOWMAXIMIZEDCHANGE")}
//  , { 803   , _T("WM_DWMSENDICONICTHUMBNAIL")}
//  , { 806   , _T("WM_DWMSENDICONICLIVEPREVIEWBITMAP")}
//  , { 831   , _T("WM_GETTITLEBARINFOEX")}
//  , { 856   , _T("WM_HANDHELDFIRST")}
//  , { 863   , _T("WM_HANDHELDLAST")}
//  , { 864   , _T("WM_AFXFIRST")}
//  , { 895   , _T("WM_AFXLAST")}
//  , { 896   , _T("WM_PENWINFIRST")}
//  , { 911   , _T("WM_PENWINLAST")}
//  , { 32768 , _T("WM_APP")}
//  , { 1024  , _T("WM_USER")}
//  , { 32    , _T("WM_NCCALCSIZE")}
//  , { 1024  , _T("WM_WINDOWPOSCHANGING")}
//  , { 4     , _T("WM_ERASEBACKGROUND")}
//  , { 256   , _T("WM_ENTERIDLE")}
//  , { 128   , _T("WM_CHAR")}