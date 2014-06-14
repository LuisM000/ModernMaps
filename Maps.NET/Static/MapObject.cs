using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Maps.NET.Static
{
    class MapObject
    {
        public static List<Pushpin> pushpinPlaces { get; set; }
        public static Pushpin pusphinME { get; set; }

        public static void removeMapPushpin(Map myMap, string tag)
        {
            List<Pushpin> elementsToRemove = new List<Pushpin>();
            foreach (UIElement element in myMap.Children)
            {
                if (element.GetType() == typeof(Pushpin))
                {
                    Pushpin pin = (Pushpin)element;
                    if (pin != null)
                    {
                        if (pin.Tag!=null && pin.Tag.ToString() == tag)
                        {
                            elementsToRemove.Add((Pushpin)element);
                        }
                    }
                }
            }
            foreach (UIElement element in elementsToRemove)
            {
                myMap.Children.Remove(element);
            }
        }

       public static void removeRoute(Map myMap,string tag)
        {
            List<MapPolyline> elementsToRemove = new List<MapPolyline>();
            List<MapLayer> elementsToRemove2 = new List<MapLayer>();
            foreach (UIElement element in myMap.Children)
            {
                if (element.GetType() == typeof(MapPolyline))
                {
                    MapPolyline pin = (MapPolyline)element;
                    if (pin != null)
                    {
                        if (pin.Tag != null && pin.Tag.ToString() == tag)
                        {
                            elementsToRemove.Add((MapPolyline)element);
                        }
                    }
                }
                if (element.GetType() == typeof(MapLayer))
                {
                    MapLayer pin = (MapLayer)element;
                    if (pin != null)
                    {
                        if (pin.Tag != null && pin.Tag.ToString() == tag)
                        {
                            elementsToRemove2.Add((MapLayer)element);
                        }
                    }
                }
            }
            foreach (UIElement element in elementsToRemove)
            {
                myMap.Children.Remove(element);
            }
            foreach (UIElement element in elementsToRemove2)
            {
                myMap.Children.Remove(element);
            }
        }
    }
}
