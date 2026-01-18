namespace Resources.Script.UI
{
    public abstract class Presenter<TView, TModel> where TView : IView
    {
        protected TView view;
        protected TModel model;

        public Presenter(TView view, TModel model)
        {
            this.view = view;
            this.model = model;
        }
        
        public abstract void Init();
        public abstract void Release();
    }
}