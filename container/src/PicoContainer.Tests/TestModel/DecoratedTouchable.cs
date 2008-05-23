namespace PicoContainer.TestModel
{
    /// <summary>
    /// Summary description for DecoratedTouchable.
    /// </summary>
    public class DecoratedTouchable : ITouchable
    {
        private readonly ITouchable theDelegate;

        public DecoratedTouchable(ITouchable theDelegate)
        {
            this.theDelegate = theDelegate;
        }

        #region ITouchable Members

        public bool WasTouched
        {
            get { return theDelegate.WasTouched; }
        }

        public void Touch()
        {
            theDelegate.Touch();
        }

        #endregion
    }
}