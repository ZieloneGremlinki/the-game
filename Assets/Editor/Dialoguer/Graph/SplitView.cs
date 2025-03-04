using UnityEngine.UIElements;

namespace GreenGremlins.Dialoguer.Editor
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits>
        {
        }
    }
}