using System;
using System.Drawing;
using MonoTouch.UIKit;

using bit.projects.iphone.chromatictuner.images;

namespace bit.projects.iphone.chromatictuner
{
    public interface IStaticGeometry
    {
        RectangleF mode_switch_slider { get; }                   
        RectangleF notelock_slider { get; }                   
        RectangleF cents_dial_cutout { get; }   
        RectangleF scale_tape_cutout  { get; }   
        RectangleF notelock_switch_cutout { get; }   
        RectangleF mode_switch_cutout { get; }   
        RectangleF notelock_btn_l { get; }   
        RectangleF pitchpipe_btn  { get; }   
        RectangleF tuning_btn { get; }   
        RectangleF power_btn { get; }   
        RectangleF headphone_btn { get; }   
        RectangleF settings_btn { get; }   
    }

    public class StaticGeometry_480h : IStaticGeometry
    {
        public RectangleF mode_switch_slider { get { return SvgObjectData.mode_switch_slider; } }                  
        public RectangleF notelock_slider { get { return SvgObjectData.notelock_slider; } }                
        public RectangleF cents_dial_cutout { get { return SvgObjectData.cents_dial_cutout; } }    
        public RectangleF scale_tape_cutout  { get { return SvgObjectData.scale_tape_cutout; } }   
        public RectangleF notelock_switch_cutout { get { return SvgObjectData.notelock_switch_cutout; } }   
        public RectangleF mode_switch_cutout { get { return SvgObjectData.mode_switch_cutout; } } 
        public RectangleF notelock_btn_l { get { return SvgObjectData.notelock_btn_l; } }  
        public RectangleF pitchpipe_btn  { get { return SvgObjectData.pitchpipe_btn; } }   
        public RectangleF tuning_btn { get { return SvgObjectData.tuning_btn; } }    
        public RectangleF power_btn { get { return SvgObjectData.power_btn; } }   
        public RectangleF headphone_btn { get { return SvgObjectData.headphone_btn; } }   
        public RectangleF settings_btn { get { return SvgObjectData.settings_btn; } }   
    }

    public class StaticGeometry_568h : IStaticGeometry
    {
        public RectangleF mode_switch_slider { get { return SvgObjectData.mode_switch_slider; } }                  
        public RectangleF notelock_slider { get { return SvgObjectData.notelock_slider; } }                
        public RectangleF cents_dial_cutout { get { return SvgObjectData.cents_dial_cutout_568h; } }    
        public RectangleF scale_tape_cutout  { get { return SvgObjectData.scale_tape_cutout_568h; } }   
        public RectangleF notelock_switch_cutout { get { return SvgObjectData.notelock_switch_cutout_568h; } }   
        public RectangleF mode_switch_cutout { get { return SvgObjectData.mode_switch_cutout_568h; } } 
        public RectangleF notelock_btn_l { get { return SvgObjectData.notelock_btn_l_568h; } }  
        public RectangleF pitchpipe_btn  { get { return SvgObjectData.pitchpipe_btn_568h; } }   
        public RectangleF tuning_btn { get { return SvgObjectData.tuning_btn_568h; } }    
        public RectangleF power_btn { get { return SvgObjectData.power_btn_568h; } }   
        public RectangleF headphone_btn { get { return SvgObjectData.headphone_btn_568h; } }   
        public RectangleF settings_btn { get { return SvgObjectData.settings_btn_568h; } }   
    }

    public static class StaticGeometryExtensions
    {
        public static IStaticGeometry CreateForScreen (UIScreen screen)
        {
            if (screen.IsWideScreen ()) {
                return new StaticGeometry_568h();
            }
            return new StaticGeometry_480h();
        }
    }
}

