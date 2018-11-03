using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kata.Properties;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;


namespace Kata
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for KataWindow.xaml
    /// </summary>
    public partial class KataWindow : Window
    {
        private enum KataTypes : byte
        {
            Drawabox,
            Music
        };

        private enum KataLessons : byte
        {
            Basics,
            Forms,
            Plants,
            Insects,
            Animals,
            Objects
        };
       
        // loaded from file (when resume button pressed)
        private Queue<int> _resumeSelections = new Queue<int>(); 
        // saved to file (when hits bottom of species tree)
        private Queue<int> _saveSelections = new Queue<int>();

        private bool _resuming = false;
        
        public KataWindow()
        {
            InitializeComponent();
        }
        
                private void PopulateList(string filePath)
        {
            dynamic katas = LoadYaml(filePath);
            JArray array = JArray.Parse(katas.Lessons.List.ToString());
            foreach (var item in array) {
                ComboBox_Lesson.Items.Add(item);
            }
        }

        private static string GetPythonPath()
        {

            using (StreamReader reader = new StreamReader(@"Configuration\Helper_Code\python_path.txt")) {
                string line = "";
                return (line = reader.ReadLine()) != null ? line : "";
            }
        }
        private string LoadYamlToJsonPython(string yamlFileName)
        {
            const string pythonFileName = @"Configuration\Helper_Code\YamlToJson.py";

            Process process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = GetPythonPath(),
                    Arguments = $"{pythonFileName} {yamlFileName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            // object initializer

            process.Start();

            string jsonString = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (error.Length > 0) {
                MessageBox.Show(error);
            }

            process.WaitForExit();


            return jsonString;
        }

        private static dynamic LoadYaml(string file)
        {
            using (StreamReader r = new StreamReader(file)) {
                string yamlString = r.ReadToEnd();

                var yamlObject = new DeserializerBuilder().Build().Deserialize(
                    new StringReader(yamlString)
                );

                //var jsonString = LoadYamlToJsonPython(file);
                // Old YamlDotNet serializer, needs support for Aliases in JSON
                //
                
                /*
                var serializer = new SerializerBuilder()
                    .JsonCompatible()
                    .WithMaximumRecursion(200)
                    .Build();

                var jsonString = serializer.Serialize(yamlObject);
                */
                
                
                var serializer = new Newtonsoft.Json.JsonSerializer();
                var jsonStringWriter = new StringWriter();
                serializer.Serialize(jsonStringWriter, yamlObject);
                var jsonString = jsonStringWriter.ToString();
               
                
                var jsonObject = JObject.Parse(jsonString);
                return jsonObject;
            }
        }

        private void SaveSelections()
        {
            FileStream fs = null;
            try {
                fs = new FileStream("Configuration\\selections.txt", FileMode.Create);
                using (StreamWriter writer = new StreamWriter(fs)) {
                    while (_saveSelections.Count > 0) {
                        writer.WriteLine(_saveSelections.Dequeue().ToString());
                    }
                }
            } finally {
                fs?.Dispose();
            }

            MessageBox.Show(Properties.Resources.ResourceManager.GetString(
                "String_PartialTaxonomySelected"));
            Button_Resume.Visibility = Visibility.Visible;
            ResetQueues();
        }

        private void ReadSelections()
        {
            ResetQueues();
            using (StreamReader reader = new StreamReader("Configuration\\selections.txt")) {
                string line = "";
                while ((line = reader.ReadLine()) != null) {
                    if (int.TryParse(line, out var lineValue)) {
                        _resumeSelections.Enqueue(lineValue);
                    } else {
                        throw new Exception("Error parsing line from selections.txt!");
                    }
                }
            }

            Button_Resume.Visibility = Visibility.Hidden;
        }

        private void ResetQueues()
        {
            // loaded from file (when resume button pressed)
            _resumeSelections = new Queue<int>();
            // saved to file (when hits bottom of species tree)
            _saveSelections = new Queue<int>();
        }

        /*
         *
         *
         * 
         * Private Static Methods
         *
         *
         * 
         */
        
        // generate a random number from 0 to max, inclusive that is [0, max]
        private static int RandomNumber(int max)
        {
            Random rng = new Random();
            return rng.Next(0, max + 1);
        }

        private static int SelectLessonNum(dynamic katas)
        {
            int numLessons = katas.Lessons.Content.Count;
            return RandomNumber(numLessons - 1);
        }

        private static int SelectExerciseNum(dynamic lesson)
        {
            int numExercises = lesson.Exercises.Count;
            int exerciseNum = RandomNumber(numExercises - 1);
            return exerciseNum;
        }

        private static dynamic SelectEverydayObject(int exerciseNum)
        {
            const string everydayObjectsFile = "Configuration\\YAML\\EverydayObjects.yaml";
            dynamic everydayObjects = LoadYaml(everydayObjectsFile);

            dynamic everydayObjectGroup = everydayObjects["Everyday Objects"][exerciseNum];
            int exerciseCount = everydayObjectGroup.Content.Count;

            int subExerciseNum = RandomNumber(exerciseCount - 1);
            return everydayObjectGroup.Content[subExerciseNum];
        }

        /*
         *
         * 
         * Private methods
         *
         * 
         */
        
        private dynamic GetAnimaliaPhylum(dynamic phyla, int lessonNum)
        {
            dynamic selectedPhylum = null;
            int selectedPhylumIndex = 0;

            if (_resuming && _resumeSelections.Count > 0) {
                selectedPhylumIndex = _resumeSelections.Dequeue();
                selectedPhylum = phyla[selectedPhylumIndex];
            } else {
                _resuming = false;
                int i = 0;
                foreach (dynamic phylum in phyla) {
                    if (phylum.Taxon_Name == "Chordata" &&
                        lessonNum == (byte)KataLessons.Animals
                    ) {
                        selectedPhylum = phylum;
                        selectedPhylumIndex = i;
                    } else if (phylum.Taxon_Name == "Arthropoda" &&
                        lessonNum == (byte)KataLessons.Insects
                    ) {
                        selectedPhylum = phylum;
                        selectedPhylumIndex = i;
                    }
                    i++;
                }

            }
            _saveSelections.Enqueue(selectedPhylumIndex);
            return selectedPhylum;
        }

        private dynamic SelectAnimal(dynamic kingdom, dynamic exercise, int exerciseNum)
        {
            dynamic phylum = GetAnimaliaPhylum(kingdom.Content.Content, (int)KataLessons.Animals);
            return DescendTaxon(phylum, "");
        }

        private dynamic SelectInsect(dynamic kingdom, dynamic exercise, int exerciseNum)
        {
            dynamic phylum = GetAnimaliaPhylum(kingdom.Content.Content, (int)KataLessons.Insects);
            return DescendTaxon(phylum, "");
        }


        private int GetTaxonNum(dynamic taxonomicRank)
        {
            int taxonNum = 0;
            if (_resuming && _resumeSelections.Count > 0) {
                taxonNum = _resumeSelections.Dequeue();
            } else {
                _resuming = false;
                taxonNum = RandomNumber(taxonomicRank.Content.Count - 1);
            }
            _saveSelections.Enqueue(taxonNum);
            return taxonNum;
        }

        private dynamic PickNextTaxon(dynamic currentTaxon, string taxonString)
        {
            dynamic taxonomicRank = currentTaxon.Content;
            if (taxonomicRank.Content != null) {
                int taxonNum = GetTaxonNum(taxonomicRank);
                dynamic taxon = taxonomicRank.Content[taxonNum];

                if (taxonomicRank.Taxonomic_Rank == "Species" || taxonomicRank["Bottom"] != null) {
                    // we're done!
                    return taxon.Taxon_Name;
                } else {
                    return DescendTaxon(taxon, taxonString);
                }
            } else {
                SaveSelections();
                return taxonString + " " + currentTaxon.Taxon_Name.ToString();
            }
        }

        private dynamic DescendTaxon(dynamic currentTaxon, string taxonString)
        {
            try {
                if (currentTaxon.Content != null) {
                    return PickNextTaxon(currentTaxon, taxonString + " " + currentTaxon.Taxon_Name.ToString());
                } else {
                    SaveSelections();
                    return taxonString + " " + currentTaxon.Taxon_Name.ToString();
                }
            } catch (Exception e) {
                MessageBox.Show("Could not descend taxon: got error " + e.InnerException
                                + " with taxon:" + currentTaxon + " and string: " + taxonString);
                return "";
            }
        }

        private dynamic SelectPlant(dynamic kingdom, dynamic exercise, int exerciseNum)
        {
            return DescendTaxon(kingdom, "");
        }

        private dynamic GetKingdom(dynamic kingdoms, int lessonNum)
        {
            dynamic selectedKingdom = null;
            int selectedKingdomIndex = 0;

            if (_resuming && _resumeSelections.Count > 0) {
                selectedKingdomIndex = _resumeSelections.Dequeue();
                selectedKingdom = kingdoms[selectedKingdomIndex];
            } else {
                _resuming = false;
                int i = 0;
                foreach (dynamic kingdom in kingdoms) {
                    if (kingdom.Taxon_Name == "Plantae" && lessonNum == (byte) KataLessons.Plants) {
                        selectedKingdom = kingdom;
                        selectedKingdomIndex = i;
                    } else if (kingdom.Taxon_Name == "Animalia" && (lessonNum == (byte) KataLessons.Animals ||
                                                                    lessonNum == (byte) KataLessons.Insects)) {
                        selectedKingdom = kingdom;
                        selectedKingdomIndex = i;
                    }

                    i++;
                }

            }
            _saveSelections.Enqueue(selectedKingdomIndex);
            return selectedKingdom;
        }

        private dynamic SelectSpecies(dynamic exercise, int lessonNum, int exerciseNum)
        {
            const string speciesFile = "Configuration\\YAML\\Species.yaml";

            try {
                File.Copy("../../" + speciesFile, speciesFile, true);
            } catch (Exception e) {
                MessageBox.Show(
                    Properties.Resources.
                        String_SelectSpeciesError_CouldNotUpdateSpeciesYaml
                    + e.InnerException);
            }

            dynamic species = LoadYaml(speciesFile);
            dynamic kingdom = GetKingdom(species.Kingdoms, lessonNum);

            switch (lessonNum) {
            case (byte)KataLessons.Plants:
                return SelectPlant(kingdom, exercise, exerciseNum);
            case (byte)KataLessons.Insects:
                return SelectInsect(kingdom, exercise, exerciseNum);
            case (byte)KataLessons.Animals:
                return SelectAnimal(kingdom, exercise, exerciseNum);
            default:

                MessageBox.Show(
                    Properties.Resources.
                        String_SelectSpeciesError_IncorrectLesson
                );
                return "";
            }
        }

        private int GetLessonNum(dynamic katas)
        {
            int lessonNum = 0;
            if (ComboBox_Lesson.SelectedIndex >= 0) { 
                // handles selecting from only one lesson, for now
                lessonNum = ComboBox_Lesson.SelectedIndex;
            } else {
                if (_resuming && _resumeSelections.Count > 0) {
                    lessonNum = _resumeSelections.Dequeue();
                } else {
                    _resuming = false;
                    lessonNum = SelectLessonNum(katas);
                }
            }
            _saveSelections.Enqueue(lessonNum);
            return lessonNum;
        }

        private int GetExerciseNum(dynamic lesson)
        {
            int exerciseNum = 0;
            if (_resuming && _resumeSelections.Count > 0) {
                exerciseNum = _resumeSelections.Dequeue();
            } else {
                _resuming = false;
                exerciseNum = SelectExerciseNum(lesson);
            }
            _saveSelections.Enqueue(exerciseNum);
            return exerciseNum;
        }

        private void PickDrawaboxExerciseRandomly()
        {
            const string kataFile = "Configuration\\YAML\\DrawaboxKatas.yaml";
            dynamic katas = LoadYaml(kataFile);

            int lessonNum = GetLessonNum(katas);
            dynamic lesson = null;
            lesson = katas.Lessons.Content[lessonNum];

            int exerciseNum = GetExerciseNum(lesson);

            dynamic exercise = null;
                
            if (exerciseNum <= lesson.Exercises.Count - 1) {
                exercise = lesson.Exercises[exerciseNum];
            } else {
                MessageBox.Show($@"Error, exercise {exerciseNum} out of bounds, size: {lesson.Exercise.Count}!");
            }
            dynamic subExercise = null;

            switch (lessonNum) {
            case (byte)KataLessons.Objects:
                subExercise = SelectEverydayObject(exerciseNum);
                break;
            case (byte)KataLessons.Animals:
            case (byte)KataLessons.Insects:
            //Kingdom Plantae exercise
            case (byte)KataLessons.Plants when exerciseNum == 2:
                subExercise = SelectSpecies(exercise, lessonNum, exerciseNum);
                break;
            // ReSharper disable once RedundantEmptySwitchSection
            default:
                // lesson has no sub-exercise
                break;
            }

            string messageExercise = string.Format("Lesson: {0}\nExercise: {1}", lesson.Name, exercise);
            string messageSubexercise = "";

            if (subExercise != null) {
                messageSubexercise = string.Format("\nSubexercise: {0}", subExercise);
            }

            string message = messageExercise + messageSubexercise;

            Label_Result.Content = message;
        }

        private void PickMusicExerciseRandomly()
        {
            const string musicKataFile = "Configuration\\YAML\\Music.yaml";
            dynamic musicKatas = LoadYaml(musicKataFile);

            int bpmIndex = RandomNumber(musicKatas.BPM.Count - 1);
            int bpm = musicKatas.BPM[bpmIndex];

            int keyIndex = RandomNumber(musicKatas.Key.Count - 1);
            string key = musicKatas.Key[keyIndex];

            string message = $"BPM - {bpm}\nKey - {key}";

            Label_Result.Content = message;
        }

        private void ButtonRandomlySelect_Click(object sender, EventArgs e)
        {
            switch (ComboBox_KataType.SelectedIndex) {
            case (byte)KataTypes.Drawabox:
                PickDrawaboxExerciseRandomly();
                break;
            case (byte)KataTypes.Music:
                PickMusicExerciseRandomly();
                break;
            default:
                Label_Result.Content = Properties.Resources.String_Error_NoKatTypeSelected;
                break;

            }
        }

        private void ComboBoxKataType_SelectionChanged(object sender, EventArgs e)
        {
            if (ComboBox_Lesson != null) {
                if (ComboBox_KataType.SelectedIndex == (byte)KataTypes.Music) {
                    ComboBox_Lesson.Visibility = Visibility.Hidden;
                } else {
                    ComboBox_Lesson.Visibility = Visibility.Visible;
                    PopulateList("Configuration\\YAML\\DrawaboxKatas.yaml");
                }
                
            }

        }

        private void ResetResultText()
        {
            Label_Result.Content = Properties.Resources.String_SelectedKata;
        }
        private void Reset()
        {
            ComboBox_KataType.SelectedIndex = -1;
            ComboBox_KataType.Text = Properties.Resources.String_KataType;

            ComboBox_Lesson.SelectedIndex = -1;
            ComboBox_Lesson.Text = Properties.Resources.String_OnlyOneLesson;

            ResetResultText();
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void ButtonResume_Click(object sender, EventArgs e)
        {
            ResetResultText();
            ReadSelections();
            _resuming = true;
            PickDrawaboxExerciseRandomly();
        }
        
    }
}
