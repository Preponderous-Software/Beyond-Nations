namespace osg {
    public abstract class InfoBox {
        protected int x;
        protected int y;
        protected int width;
        protected int height;
        protected int padding;
        protected string title;

        public InfoBox(int x, int y, int width, int height, int padding, string title) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.padding = padding;
            this.title = title;
        }

        public abstract void draw();
    }
}