using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Net;
using System.IO;

namespace CombinarHorariosJunto
{
    class Program
    {
        static void Main(string[] args)
        {
            int creditosMin = 26;
            int NumAssignatruas = 5;
            Assignatura Unidad = new Assignatura();
            List<Assignatura> ListAss = new List<Assignatura>();
            Condiciones Cond1 = new Condiciones();

            List<string> grupo;
            List<string> grupoCodigo;
            //Añadir assignaturas que se tienen que combinar en una lista
            grupo = new List<string>() { "4T11", "4T12", "4T21" };
            grupoCodigo = new List<string>() { "125-1", "125-2", "130-1" };
            Unidad = InsertarAss("ER", "EMISSORS I RECEPTORS (ER)", 4.5, "300029",grupo,grupoCodigo);
            ListAss.Add(Unidad);
            Cond1.AssCorequisit.Add(Unidad);
            
            grupo = new List<string>() { "5M21" };
            grupoCodigo = new List<string>() { "145-1" };
            Unidad = InsertarAss("MXS", "MOBILITAT, XARXES I SERVEIS (MXS)", 6, "300043", grupo, grupoCodigo);
            ListAss.Add(Unidad);
            Cond1.Assignatura = Unidad;
            
            grupo = new List<string>() { "5M21" };
            grupoCodigo = new List<string>() { "145-1" };
            Unidad = InsertarAss("SAI", "SERVEIS AUDIOVISUALS SOBRE INTERNET (SAI)", 4, "300041", grupo, grupoCodigo);
            ListAss.Add(Unidad);
            
            grupo = new List<string>() { "7T11", "7T21" };
            grupoCodigo = new List<string>() { "240-1", "245-1" };
            Unidad = InsertarAss("TIQ", "TECNOLOGIES D'INFORMACIÓ QUÀNTICA (TIQ)", 6, "300050", grupo, grupoCodigo);
            ListAss.Add(Unidad);
            
            grupo = new List<string>() { "6M11" };
            grupoCodigo = new List<string>() { "190-1" };
            Unidad = InsertarAss("EA", "ENGINYERIA D'APLICACIONS (EA-M)", 12, "300045", grupo, grupoCodigo);
            ListAss.Add(Unidad);
            
            grupo = new List<string>() { "6PX1" };
            grupoCodigo = new List<string>() { "200-1" };
            Unidad = InsertarAss("GEO-MP3", "GEOTÈCNIA (GEO-MP3)", 4.5, "300240", grupo, grupoCodigo);
            ListAss.Add(Unidad);
            
            grupo = new List<string>() { "6PX1", "6PX2" };
            grupoCodigo = new List<string>() { "200-1", "200-2" };
            Unidad = InsertarAss("IE", "INSTAL·LACIONS ELÈCTRIQUES (IE-MP4)", 6, "300237", grupo, grupoCodigo);
            ListAss.Add(Unidad);
            
            grupo = new List<string>() { "6PX1" };
            grupoCodigo = new List<string>() { "200-1" };
            Unidad = InsertarAss("PPA", "PLANIFICACIÓ I PROCESSOS AEROPORTUARIS (PPA-MP5)", 6, "300253", grupo, grupoCodigo);
            ListAss.Add(Unidad);
            
            grupo = new List<string>() { "6PX1" };
            grupoCodigo = new List<string>() { "200-1" };
            Unidad = InsertarAss("TE", "TEORIA D'ESTRUCTURES (TE-MP6)", 6, "300234", grupo, grupoCodigo);
            ListAss.Add(Unidad);

            //Crear listas de las assignatruas que tienen requisitos y corequisitos
            List<Condiciones> ListCond = new List<Condiciones>();
            ListCond.Add(Cond1);


            //Llamar la funcion que devuelve una lista de listas de assingaturas {[DSA,IX],[DSA,IOT],[IX,IOT]}
            List<List<Assignatura>> listaCombinaciones = Combinaciones(ListAss, NumAssignatruas, creditosMin, ListCond);
            List<string> listaCodigosT;
            int f = 0;
            foreach (List<Assignatura> listAss in listaCombinaciones)
            {
                string path = @"C:\Users\calca\Downloads\Horarios" + f.ToString();
                Directory.CreateDirectory(path);
                listaCodigosT = CombinarNAsignturas(listAss);
                
                ComprobarSolapesYDescgargar(listaCodigosT,f);
                f++;

            }
            
        }

        public class Assignatura
        {
            public string Siglas;
            public string Nombre;
            public string NombreCodigo;
            public double creditos;
            public List<String> subgrupos;
            public List<String> subgruposCodigo;


        }
        public class Condiciones
        {
            public Assignatura Assignatura;
            public List<Assignatura> AssCorequisit = new List<Assignatura>();
        }
        public static Assignatura InsertarAss(string S, string N, double C, string NombreCodigo, List<String> subgrupos, List<String> subgruposCodigo)
        {
            Assignatura A = new Assignatura();
            A.Siglas = S;
            A.Nombre = N;
            A.creditos = C;
            A.NombreCodigo = NombreCodigo;
            A.subgrupos = subgrupos;
            A.subgruposCodigo = subgruposCodigo;
            return A;
        }
        public static Condiciones InsertarCond(Assignatura A, List<Assignatura> C)
        {
            Condiciones Cond = new Condiciones();
            Cond.Assignatura = A;
            Cond.AssCorequisit = C;
            return Cond;

        }
        public static List<List<Assignatura>> Combinaciones(List<Assignatura> ListAssF, int NumAssignatruas, int creditosMin, List<Condiciones> ListCond)
        {
            // El algoritmo de combinacion es
            // 1000      Primer bloque        
            // 0100
            // 0010
            // 0001

            // 1000     Segundo bloque
            // 1100     En todos los grupos anteriores (lineas anterories)
            // 1010       se añade el elemento de la primera columna
            // 1001
            // 1100

            // 0100     Segundo bloque
            // 0110     En todos los grupos anteriores (lineas anterories)
            // 0101       se añade el elemento de la segunda columna
            // 1100
            // 1100
            // 1110
            // 1101

            // 1010
            // 0110
            // 0010     Elemento repetido 3ra columna
            // 0011
            // 1010
            // 1110
            // 1010     Elemento repetido 3ra columna
            // 1011
            // 1110
            // 0110
            // 0110
            // 0111
            // 1110
            // 1110
            // 1110     Elemento repetido 3ra columna
            // 1111


            List<List<Assignatura>> CombinacionesAss = new List<List<Assignatura>>();
            List<List<Assignatura>> CombinacionesAssF = new List<List<Assignatura>>();

            //Se añade el primer bloque
            for (int i = 0; i < ListAssF.Count; i++)
            {
                List<Assignatura> L = new List<Assignatura>();
                L.Add(ListAssF[i]);
                CombinacionesAss.Add(L);
            }

            //Se hacen los siguientes bloques del algoritmo de combinacion
            foreach (Assignatura j in ListAssF)
            {
                int l = 0;
                int indice = CombinacionesAss.Count;
                while (l < indice)
                {
                    //En los bloques hay lineas en la que el elemento de la columna ya esta, entonces ese lo saltamos
                    //para ello miramos en cada fila si el elemento de la columna se encuentra en la fila
                    bool encontrado = false;
                    foreach (Assignatura k in CombinacionesAss[l])
                    {
                        if (k == j)
                        {
                            encontrado = true;
                        }
                    }

                    //Si no se encuentra el elmento se procede a añadir las nueva lineas de combinacion
                    if (!encontrado)
                    {
                        Assignatura[] B = new Assignatura[CombinacionesAss[l].Count + 1];
                        int d = 0;
                        foreach (Assignatura m in CombinacionesAss[l])
                        {
                            B[d] = m;


                            d++;
                        }

                        B[d] = j;

                        double sum = 0;

                        //Para sumar el total de creditos en la combinacion
                        foreach (Assignatura x in B)
                        {
                            sum = sum + x.creditos;
                        }

                        //Agrega solo si cumple las dos condiciones
                        if ((B.Length == NumAssignatruas) && (sum >= creditosMin))
                        {
                            List<Assignatura> LAF = new List<Assignatura>();
                            foreach (Assignatura m in B)
                            {
                                LAF.Add(m);
                            }
                            //Aqui tenemos la agregacion a la lista que contienen las combinaciones que nos interesan
                            CombinacionesAssF.Add(LAF);
                        }
                        List<Assignatura> LA = new List<Assignatura>();
                        foreach (Assignatura m in B)
                        {
                            LA.Add(m);
                        }
                        //Aqui tenemos la agregacion a la lista que contienen TODAS las combinaciones
                        CombinacionesAss.Add(LA);

                    }

                    l++;
                }
            }

            //Hay combinaciones que cotienen los mismos elementos pero en diferente orden
            //El WHILE  sirve para eliminar las combinaciones repetidas
            int u = 0;
            while (u < CombinacionesAssF.Count - 1)
            {
                for (int q = u + 1; q < CombinacionesAssF.Count; q++)
                {
                    int e = 0;
                    foreach (Assignatura w in CombinacionesAssF[u])
                    {
                        if (CombinacionesAssF[q].Contains(w))
                        {
                            e++;
                        }
                    }
                    if (e == NumAssignatruas)
                    {
                        CombinacionesAssF.Remove(CombinacionesAssF[q]);
                    }

                }

                u++;

            }
            //FOR para eliminar las combinaciones que no cumplen los corequisitos
            foreach (Condiciones Cond in ListCond)
            {
                int i = 0;
                while (i < CombinacionesAssF.Count)
                {
                    if (CombinacionesAssF[i].Contains(Cond.Assignatura))
                    {
                        int eliminar = 0;
                        foreach (Assignatura Ass in Cond.AssCorequisit)
                        {
                            if (CombinacionesAssF[i].Contains(Ass))
                            {
                                eliminar++;
                            }
                        }
                        if (eliminar != Cond.AssCorequisit.Count)
                        {
                            CombinacionesAssF.Remove(CombinacionesAssF[i]);
                        }
                        else
                        {
                            i++;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            return CombinacionesAssF;


        }

        public static void ComprobarSolapesYDescgargar(List<string> listaCodigosT,int f)
        {
            WebClient mywebClient = new WebClient();
            
            for (int i = 0; i < listaCodigosT.Count; i++)
            {
                mywebClient.DownloadFile("https://sia.upc.edu/SIA/INFOWEB.horariAssigsGrups_CSV?a_assigs_grups=" + listaCodigosT[i] + "@&v_curs_quad=2022-2&a_codi_programes=@@", @"C:\Users\calca\Downloads\Horarios" + f + "/Horario_" + i + ".csv");

                string ubicacionArchivo = @"C:\Users\calca\Downloads\Horarios" + f + "/Horario_" + i + ".csv";
                System.IO.StreamReader archivo = new System.IO.StreamReader(ubicacionArchivo);

                List<List<string>> LineasT = new List<List<string>>();
                // Si el archivo no tiene encabezado, elimina la siguiente línea
                archivo.ReadLine(); // Leer la primera línea pero descartarla porque es el encabezado
                string linea = archivo.ReadLine();
                while (linea != null)
                {
                    string[] LineasF = linea.Split('"');
                    LineasT.Add(LineasF.ToList<string>());
                    linea = archivo.ReadLine();
                }
                int combinacion = 1;
                int j = 0;
                while ((j < LineasT.Count - 1) && (combinacion == 1))
                {

                    int k = j + 1;
                    while ((k < LineasT.Count) && (combinacion == 1))
                    {
                        if (LineasT[j][3] == LineasT[k][3])
                        {
                            int Hinicioj = Convert.ToInt32(LineasT[j][5].Split(':')[0]);
                            int Minicioj = Convert.ToInt32(LineasT[j][5].Split(':')[1]);
                            int IJ = Hinicioj * 100 + Minicioj;
                            int Hfinalj = Convert.ToInt32(LineasT[j][9].Split(':')[0]);
                            int Mfinalj = Convert.ToInt32(LineasT[j][9].Split(':')[1]);
                            int FJ = Hfinalj * 100 + Mfinalj;

                            int Hiniciok = Convert.ToInt32(LineasT[k][5].Split(':')[0]);
                            int Miniciok = Convert.ToInt32(LineasT[k][5].Split(':')[1]);
                            int IK = Hiniciok * 100 + Miniciok;
                            int Hfinalk = Convert.ToInt32(LineasT[k][9].Split(':')[0]);
                            int Mfinalk = Convert.ToInt32(LineasT[k][9].Split(':')[1]);
                            int FK = Hfinalk * 100 + Mfinalk;

                            if (FJ != 0)
                            {
                                if ((IJ > IK) && (IJ < FK))
                                    combinacion = 0;

                                else if (IJ == IK)
                                    combinacion = 0;
                                else if ((IJ < IK) && (FJ > IK))
                                    combinacion = 0;
                                else
                                    k++;
                            }
                            else
                                k++;
                        }
                        else
                            k++;
                    }
                    j++;
                }
                if (combinacion == 1)
                {
                    mywebClient.DownloadFile("https://sia.upc.edu/SIA/INFOWEB.horariAssigsGrups?a_assigs_grups=" + listaCodigosT[i] + "@&n_hora_ini=&n_hora_fi=&v_curs_quad=2022-2&a_codi_programes=@@", @"C:\Users\calca\Downloads\Horarios" + f + "/Horario_" + i + ".pdf");


                }
                





            }
        }
        public static List<string> CombinarNAsignturas(List<Assignatura>  listAss)
        {
            List<string> CodiogsExcel = new List<string>();
            foreach (Assignatura ass in listAss)
            {
                
                int z = 0;
               
                int indice = CodiogsExcel.Count;
                if (CodiogsExcel.Count != 0)
                {
                    foreach (var j in ass.subgruposCodigo)
                    {

                        for (int k = 0; k < indice; k++)
                        {
                            CodiogsExcel.Add(CodiogsExcel[k] + "@" + ass.NombreCodigo + "-" + j);
                        }
                    }
                }
                else
                {
                    foreach (var j in ass.subgruposCodigo)
                    {
                        CodiogsExcel.Add("@" + ass.NombreCodigo + "-" + j);

                    }
                }




            }
            int l = 0;
            while (l < CodiogsExcel.Count())
            {
                if (CodiogsExcel[l].Count(f => f == '@') != listAss.Count)
                    CodiogsExcel.RemoveAt(l);
                else
                    l++;
            }
            return CodiogsExcel;
        }

    }


}
