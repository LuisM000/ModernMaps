using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maps.NET.Static
{
    class StaticAdvices
    {
        public static List<string> Advices { get; set; }

        private static int index;

        private static void loadAdvices()
        {
            Advices = new List<string>();
            Advices.Add("Pulsa el botón derecho encima del mapa para crear un marcador adicional y calcular la ruta hasta él, establecerlo como posición actual o buscar locales cercanos");
            Advices.Add("Cambia el tipo de mapa para ver vista aérea en la pestaña Básico, Vista aérea");
            Advices.Add("No pierdas detalle del mapa. Ve a la pestaña básico y desmarca las opciones Ver todo y Maximizar barra, para que el mapa tenga prioridad");
            Advices.Add("Guarda en el portapapeles las coordenadas de la posición actual haciendo clic en el botón Datos posición, de la pestaña básico");
            Advices.Add("¿Quieres saber cómo se obtiene la información de Google Maps? Pulsa en Request (dentro de la pestaña básico) y podrás ver todas las peticiones que se han hecho. Copia una petición y búscala con tu navegador favorito");
            Advices.Add("Envíanos tus sugerencias a través de la pestaña Básico, botón Feedback");
            Advices.Add("Quieres evitar peajes en tus rutas. Configura tus opciones en la sección de rutas");
            Advices.Add("Ordena tus lugares por relevancia, distancia, valoración u horario, para así ajustarlo a tus preferencias");
            Advices.Add("Tienes muchas opciones de personalizar tus mapas estáticos. modifica los valores de gamma y brillo para crear mapas increíbles");
        }

        public static string getRandomAdvice()
        {
            if(Advices==null){loadAdvices();}
            string returnString = Advices[index];
            checkIndex();
            return returnString;
        }

        private static void checkIndex()
        {
            index = (index + 1 > Advices.Count - 1) ? 0 : index + 1;
        }
    }
}
