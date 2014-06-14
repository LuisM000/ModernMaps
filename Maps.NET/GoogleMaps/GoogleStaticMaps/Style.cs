using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maps.NET.GoogleMaps.GoogleStaticMaps
{
    class Style
    {
        public enum FeatureStyle
        {
            all, administrative, administrative__country, administrative__land_parcel, administrative__locality, administrative__neighborhood, administrative__province, landscape, landscape__man_made, landscape__natural, landscape__natural__landcover, landscape__natural__terrain, poi, poi__attraction, poi__business, poi__government, poi__medical, poi__park, poi__place_of_worship, poi__school, poi__sports_complex, road, road__arterial, road__highway, road__highway__controlled_access, road__local, transit, transit__line, transit__station, transit__station__airport, transit__station__bus, transit__station__rail, water
        }
        public enum ElementStyle { 
            all,geometry,labels
        }

        public FeatureStyle Feature { get; set; }
        public ElementStyle Element{get;set;}
        public RulesStyle Rule { get; set; }


        public static string getStyle(List<Style> styles)
        {
            string stylesReturn = "";

            if(styles!=null && styles.Count>0)
            {
                foreach (var item in styles)
	            {
                    stylesReturn += getStyle(item);
	            }
            }

            return stylesReturn;
        }
        public static string getStyle(Style style)
        {
            string styleReturn = "";
            if(style!=null)
            {
                styleReturn = "&style=" + getFeature(style.Feature) + getElement(style.Element) + RulesStyle.getRule(style.Rule);

            }
            return styleReturn;
        }

        private static string getFeature(Style.FeatureStyle resource)
        {
            return "feature:" + resource.ToString().Replace("__", ".");
        }
        private static string getElement(Style.ElementStyle element)
        {
            return "|element:" + element.ToString();
        }

        public class RulesStyle
        {
            public enum ColorRule { standard, black, brown, green, purple, yellow, blue, gray, orange, red, white }
            public enum VisibilityRule { on, off, simplified }

            public ColorRule Hue { get; set; }
            public int Lightness { get; set; }
            public int Saturation { get; set; }
            public double Gamma { get; set; }
            public bool InverseLightness { get; set; }
            public VisibilityRule Visibility { get; set; }


            public static string getRule(RulesStyle rule)
            {
                string ruleString = "";
                string hue, lightness, saturation, gamma, inverseLightness, visibility;

                hue = (rule.Hue != ColorRule.standard) ? "|hue:" + rule.Hue.ToString() : "";
                lightness = "|lightness:" + rule.Lightness;
                saturation = "|saturation:" + rule.Saturation;
                gamma = (rule.Gamma == 0) ? "|gamma:1" : "|gamma:" + rule.Gamma.ToString("0.00");
                inverseLightness = (rule.InverseLightness) ? "|inverse_lightness:true" : "";
                visibility = "|visibility:" + rule.Visibility.ToString();
                ruleString = hue + lightness + saturation + gamma + inverseLightness + visibility;

                return ruleString;
            }
        }
       
    }
}
