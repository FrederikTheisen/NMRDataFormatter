using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MDTools
{
    public class TalosToFlexPepDockFormatter
    {
        static List<Residue> Residues = new List<Residue>();
        int FirstID;
        string Sequence;

        public TalosToFlexPepDockFormatter()
        {
        }

        public static void Run()
        {
            var file = File.ReadAllLines("AngleRestraintData.txt");

            ReadData(file);

            Output();
        }

        static void ReadData(string[] file)
        {
            foreach (var line in file)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var data = line.Replace('.', ',').Split();

                if (data[0] == "REMARK") continue;

                if (data[0] == "DATA")
                {
                    continue;
                }

                if (data[0] == "VARS") continue;
                if (data[0] == "FORMAT") continue;

                Residues.Add(new Residue(data));
            }
        }

        /// <summary>
        /// Dihedral CG 13 CD2 13 NE2 13 ZN 32 CIRCULARHARMONIC 3.14 0.35
        /// </summary>
        static void Output()
        {
            foreach (var res in Residues)
            {
                if (res.ShouldIgnore) continue;

                Console.WriteLine(res.GetPhiLine());
                Console.WriteLine(res.GetPsiLine());
            }
        }

        class Residue
        {
            int ResID;

            public bool ShouldIgnore => PhiE < 0.001f || PsiE < 0.001f;

            float Phi = -9999;
            float PhiE = 0;

            float Psi = -9999;
            float PsiE;

            public Residue(string[] data)
            {
                data = data.Where(v => !string.IsNullOrEmpty(v)).ToArray();

                ResID = int.Parse(data[0]);

                Phi = float.Parse(data[2]);
                Psi = float.Parse(data[3]);
                PhiE = float.Parse(data[4]);
                PsiE = float.Parse(data[5]);
            }

            Tuple<int, string>[] GetPhiResidues()
            {
                return new Tuple<int, string>[]
                {
                    new Tuple<int, string>(ResID-1,"C"),
                    new Tuple<int, string>(ResID, "N"),
                    new Tuple<int, string>(ResID, "CA"),
                    new Tuple<int, string>(ResID, "C"),
                };
            }

            Tuple<int, string>[] GetPsiResidues()
            {
                return new Tuple<int, string>[]
                {
                    new Tuple<int, string>(ResID,"N"),
                    new Tuple<int, string>(ResID, "CA"),
                    new Tuple<int, string>(ResID, "C"),
                    new Tuple<int, string>(ResID + 1, "N"),
                };
            }

            string ResIDName(Tuple<int, string> t) => ResIDName(t.Item1, t.Item2);
            string ResIDName(int id, string name) => " " + name + " " + id;

            string GetNameLine(bool phi = true)
            {
                Tuple<int, string>[] tuple = new Tuple<int, string>[0];

                if (phi) tuple = GetPhiResidues();
                else tuple = GetPsiResidues();

                //Dihedral CG 13 CD2 13 NE2 13 ZN 32 CIRCULARHARMONIC 3.14 0.35
                return "Dihedral"
                    + ResIDName(tuple[0])
                    + ResIDName(tuple[1])
                    + ResIDName(tuple[2])
                    + ResIDName(tuple[3])
                    + " CIRCULARHARMONIC ";
            }

            public string GetPhiLine()
            {
                var line = GetNameLine(true);

                line += Phi.ToString("##0.0").Replace(',', '.');
                line += " ";
                line += PhiE.ToString("##0.0").Replace(',', '.');

                return line;
            }

            public string GetPsiLine()
            {
                var line = GetNameLine(false);

                line += Psi.ToString("##0.0").Replace(',', '.');
                line += " ";
                line += PsiE.ToString("##0.0").Replace(',', '.');

                return line;
            }
        }
    }
}
