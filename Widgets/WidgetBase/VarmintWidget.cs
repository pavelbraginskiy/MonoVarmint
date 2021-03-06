using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace MonoVarmint.Widgets
{
    
    //--------------------------------------------------------------------------------------
    /// <summary>
    /// VarmintWidget - A very simple widget class for MonoGame
    /// </summary>
    //--------------------------------------------------------------------------------------
    public partial class VarmintWidget
    {
        [VarmintWidgetInject]
        public IMediaRenderer Renderer { get; set; }

        public string Name { get; set; }
        public string Style { get; set; }
        public VarmintWidget Parent { get; set; }
        public Color BackgroundColor { get; set; }
        public bool ChildrenAffectFormatting { get; set; }
        public bool HasFocus { get; set; }
        public object Tag { get; set; }

        private Color? _foregroundColor;
        public Color ForegroundColor
        {
            get
            {
                if (_foregroundColor != null) return _foregroundColor.Value;
                if (Parent == null) return Color.Black;
                return Parent.ForegroundColor;
            }
            set { _foregroundColor = value; }
        }

        public float Opacity { get; set; } = 1.0f;

        private Vector2 _offset;
        public virtual Vector2 Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
            }
        }

        public float Rotate { get; set; }
        public bool FlipVertical { get; set; }
        public bool FlipHorizontal { get; set; }

        private Vector2? _size = null;
        internal Vector2? _originalSize = null;
        public Vector2 Size
        {
            get
            {
                return _size ?? Vector2.Zero;
            }
            set
            {
                if (_size == null || _size != value)
                {
                    if (_originalSize == null || _applyingStyles)
                    {
                        _originalSize = value;
                    }
                    _size = value;
                    UpdateChildFormatting(_size);
                    OnSizeChanged?.Invoke(this);
                }
            }
        }

        public Vector2 IntendedSize
        {
            get
            {
                if (_bindingTemplates.ContainsKey("Size")) return Size;
                return _originalSize ?? Vector2.Zero;
            }
        }

        public bool AllowInput { get; set; }
        public bool IsVisible { get; set; }
        public bool ClipToBounds { get; set; }

        private HorizontalContentAlignment? _horizontalContentAlignment;
        public HorizontalContentAlignment HorizontalContentAlignment
        {
            get
            {
                if (_horizontalContentAlignment != null) return _horizontalContentAlignment.Value;
                if (Parent == null) return HorizontalContentAlignment.Left;
                return Parent.HorizontalContentAlignment;
            }
            set { _horizontalContentAlignment = value; }
        }

        private VerticalContentAlignment? _verticalContentAlignment;
        public VerticalContentAlignment VerticalContentAlignment
        {
            get
            {
                if (_verticalContentAlignment != null) return _verticalContentAlignment.Value;
                if (Parent == null) return VerticalContentAlignment.Top;
                return Parent.VerticalContentAlignment;
            }
            set { _verticalContentAlignment = value; }
        }

        float? _fontSize;
        public float FontSize
        {
            get
            {
                if (_fontSize != null) return _fontSize.Value;
                if (Parent == null) return 0.1f;
                return Parent.FontSize;
            }
            set { _fontSize = value; }
        }


        string _fontName;
        public string FontName
        {
            get
            {
                if (_fontName != null) return _fontName;
                if (Parent == null) return null;
                return Parent.FontName;
            }
            set { _fontName = value; }
        }



        public virtual WidgetMargin Margin { get; set; }
        public virtual StretchParameter Stretch { get; set; }

        private object _content;
        public virtual object Content
        {
            get => _content;
            set
            {
                if(value is VarmintWidget)
                {
                    var widget = value as VarmintWidget;
                    widget.Parent = this;
                }
                _content = value;
            }
        }

        public bool WrapContent { get; set; }

        private object _xbindingContext;
        public object BindingContext
        {
            get
            {
                if(_xbindingContext == null)
                {
                    if (Parent == null) return null;
                    return Parent.BindingContext;
                }
                return _xbindingContext;
            }
            set
            {
                _xbindingContext = value;
                _prepared = false;
            }
        }

        private object _eventBindingContext;
        public object EventBindingContext
        {
            get
            {
                if (_eventBindingContext != null) return _eventBindingContext;
                if (Parent != null)
                {
                    var parentContext = Parent.EventBindingContext;
                    if (parentContext == null) return BindingContext;
                    else return parentContext;
                }
                return BindingContext;
            }
            set { _eventBindingContext = value; }
        }

        List<VarmintWidgetAnimation> _animations = new List<VarmintWidgetAnimation>();

        /// <summary>
        /// AbsoluteOffset
        /// </summary>
        public virtual Vector2 AbsoluteOffset
        {
            get
            {
                if (Parent == null) return Offset;
                else return Parent.AbsoluteOffset + Offset;
            }
        }


        /// <summary>
        /// Center of this widget in absolute coordinates
        /// </summary>
        public Vector2 AbsoluteCenter
        {
            get
            {
                return AbsoluteOffset + Size / 2;
            }
        }

        public Dictionary<string, string> Parameters { get; set; }

        static int _globalWidgetCount = 0;


        /// <summary>
        /// AbsoluteOpacity
        /// </summary>
        public virtual float AbsoluteOpacity
        {
            get
            {
                if (Parent == null) return Opacity;
                else return Parent.AbsoluteOpacity * Opacity;
            }
        }

        public virtual Color RenderBackgroundColor
        {
            get { return BackgroundColor * AbsoluteOpacity;  }
        }

        public virtual Color RenderForegroundColor
        {
            get { return ForegroundColor * AbsoluteOpacity; }
        }

        public virtual Color RenderGraphicColor
        {
            get { return Color.White * AbsoluteOpacity; }
        }

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// Static ctor
        /// </summary>
        //--------------------------------------------------------------------------------------
        static VarmintWidget()
        {
            _knownAssemblies.Add(typeof(VarmintWidget).GetTypeInfo().Assembly);
        }

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //--------------------------------------------------------------------------------------
        public VarmintWidget()
        {
            Parameters = new Dictionary<string, string>();
            ChildrenAffectFormatting = false;
            IsVisible = true;
            _globalWidgetCount++;
            Name = "W" + _globalWidgetCount.ToString("000000");
            AllowInput = true;
            Margin = new WidgetMargin();
            Stretch = new StretchParameter();
       }

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //--------------------------------------------------------------------------------------
        public VarmintWidget(Vector2 size) : this()
        {
            Size = size;
        }

        bool _prepared = false;
        bool _updating = false;
        bool initCalled = false;

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// Prepare 
        /// </summary>
        //--------------------------------------------------------------------------------------
        public void Prepare(Dictionary<string, VarmintWidgetStyle> styleLibrary)
        {
            if(!_prepared)
            {
                ApplyStyles(styleLibrary);
                UpdateBindings(BindingContext);
                ReadBindings();

                foreach(var child in Children)
                {
                    child.Prepare(styleLibrary);
                }

                if(!initCalled)
                {
                    initCalled = true;
                    OnInit?.Invoke(this);
                }
                _prepared = true;
                UpdateChildFormatting();
            }
        }

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// Update 
        /// </summary>
        //--------------------------------------------------------------------------------------
        public void Update()
        {
            _updating = true;
            // Load binding data first in case the init logic needs it.
            ReadBindings();
            _updating = false;
        }

    }
}