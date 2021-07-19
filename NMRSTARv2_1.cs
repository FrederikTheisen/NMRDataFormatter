using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MDTools
{
    public class NMRSTARv2_1
    {
        static string Sequence = "";
        static List<Residue> Data = new List<Residue>();

        public NMRSTARv2_1()
        {
        }

        public static void Run(string filename, string datatype = "shifty")
        {
            var file = File.ReadAllLines(filename);

            if (datatype == "shifty")
            {
                ReadShiftY(file);
            }
            else return;

            GenerateOutput();
        }

        static void ReadShiftY(string[] data)
        {
            int resn = 1;

            foreach (var line in data)
            {
                if (string.IsNullOrEmpty(line.Trim())) continue;
                if (line[0] == '#') continue;

                var dat = line.Trim().Replace('.', ',').Split().Where(d => !string.IsNullOrEmpty(d)).ToArray();

                Sequence += dat[1];

                Data.Add(new Residue(dat) { Num = resn });

                resn++;
            }
        }

        static void GenerateOutput()
        {
            List<string> lines = new List<string>
            {
                "save_entry_information",
                "\t_Saveframe_category      entry_information",
                "",
                "\t_Entry_title",
                ";",
                "PROTEIN DATA FILE. DATE: " + DateTime.Now.ToString(),
                ";",
                "",
                "save_",
                "",
                "##############################",
                "#  Polymer residue sequence  #",
                "##############################",
                "\t_Mol_residue_sequence",
                ";",
                Sequence,
                ";",
                "save_",
                "",
                "save_sample_conditions_set_1",
                "\t_Saveframe_category   sample_conditions",
                "",
                "",
                "\tloop_",
                "\t\t_Variable_type",
                "\t\t_Variable_value",
                "\t\t_Variable_value_error",
                "\t\t_Variable_value_units",
                "",
                "\t\tpH* 7   0.2  n / a",
                "\t\ttemperature  298   1    K",
                "",
                "\tstop_",
                "",
                "save_",
                "",
                "save_assigned_chemical_shifts_set_1",
                "\t_Saveframe_category               assigned_chemical_shifts",
                "",
                "\t_Sample_conditions_label         $sample_conditions_set_1",
                "\t_Mol_system_component_name       'Py J'",
                "",
                "\tloop_",
                "\t\t_Residue_seq_code",
                "\t\t_Residue_label",
                "\t\t_Atom_name",
                "\t\t_Atom_type",
                "\t\t_Chem_shift_value",
                "\t\t_Chem_shift_value_error",
                "\t\t_Chem_shift_ambiguity_code",
                ""
            };
            foreach (var res in Data)
            {
                lines.AddRange(res.GetLines());
            }
            lines.Add("");
            lines.Add("\tstop_");
            lines.Add("save_");

            foreach (var line in lines) Console.WriteLine(line);
        }


        class Residue
        {
            string ResName;
            public string ThreeLetter => OneToThreeLetterConverter.Convert(ResName);
            public int Num;

            float HA;
            float CA;
            float CB;
            float CO;
            float N;
            float HN;

            public Residue()
            {

            }

            /// <summary>
            /// #NUM	AA	HA	CA	CB	CO	N	HN
            /// </summary>
            /// <param name="data"></param>
            public Residue(string[] data)
            {
                Num = int.Parse(data[0]);
                ResName = data[1];
                HA = float.Parse(data[2]);
                CA = float.Parse(data[3]);
                CB = float.Parse(data[4]);
                CO = float.Parse(data[5]);
                N = float.Parse(data[6]);
                HN = float.Parse(data[7]);
            }

            public Residue FromShiftY(string[] data)
            {
                return new Residue
                {
                    Num = int.Parse(data[0]),
                    ResName = data[1],
                    HA = float.Parse(data[2]),
                    CA = float.Parse(data[3]),
                    CB = float.Parse(data[4]),
                    CO = float.Parse(data[5]),
                    N = float.Parse(data[6]),
                    HN = float.Parse(data[7])
                };
            }

            string ToField(string s, int l)
            {
                while (s.Length < l) s = " " + s;

                return s.Replace(',', '.');
            }

            string ToLine(string name, float value)
            {
                return ("\t\t" + Num.ToString() + "\t" + ThreeLetter + "\t" + name + "\t" + name[0] + "\t" + value.ToString("##0.000") + "\t.\t1").Replace(',','.');
            }

            public List<string> GetLines()
            {
                var lines = new List<string>();

                if (HA > 1)
                {
                    if (ResName != "G") lines.Add(ToLine("HA", HA));
                    else
                    {
                        lines.Add(ToLine("HA2", HA));
                        lines.Add(ToLine("HA3", HA));
                    }
                }
                if (CA > 1) lines.Add(ToLine("CA", CA));
                if (CB > 1) lines.Add(ToLine("CB", CB));
                if (CO > 1) lines.Add(ToLine("C", CO));
                if (N > 1) lines.Add(ToLine("N", N));
                if (HN > 1) lines.Add(ToLine("H", HN));

                return lines;
            }
        }
    }
}
