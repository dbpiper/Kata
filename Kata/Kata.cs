using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kata {
    public partial class Kata : Form {

        enum KataTypes : byte {
            Drawabox,
            Music
        };

        enum KataLessons : byte
        {
            Basics,
            Forms,
            Plants,
            Insects,
            Animals,
            Objects
        };

        Queue<int> resumeSelections = new Queue<int>(); // loaded from file (when resume button pressed)
        Queue<int> saveSelections = new Queue<int>(); // saved to file (when hits bottom of species tree)

        bool resuming = false;

        public Kata() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {

        }

        private void PopulateList(string filePath) {
            dynamic katas = LoadJson(filePath);
            JArray array = JArray.Parse(katas.Lessons.List.ToString());
            comboBoxLesson.Items.AddRange(array.ToObject<List<String>>().ToArray());
        }

        public dynamic LoadJson(string file) {
            using (StreamReader r = new StreamReader(file)) {
                string json = r.ReadToEnd();
                dynamic jsonObject = JObject.Parse(json);
                return jsonObject;
            }
        }

        private void Form1_Load(object sender, EventArgs e) {

            //comboBoxKataType.DropDownStyle = ComboBoxStyle.DropDownList;
            //comboBoxLesson.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void comboBoxLesson_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void SaveSelections()
        {
            FileStream fs = null;
            try {
                fs = new FileStream("Configuration\\selections.txt", FileMode.Create);
                using (StreamWriter writer = new StreamWriter(fs)) {
                    writer.WriteLine(saveSelections.Dequeue().ToString());
                }
            } finally {
                if (fs != null) {
                    fs.Dispose();
                }
            }
            buttonResume.Visible = true;
            ResetQueues();
        }
        
        private void ReadSelections()
        {
            ResetQueues();
            using (StreamReader reader = new StreamReader("Configuration\\selections.txt")) {
                string line = reader.ReadLine();
                int lineValue = 0;
                if (!Int32.TryParse(line, out lineValue)) {
                    resumeSelections.Enqueue(lineValue);
                } else {
                    throw new Exception("Error parsing line from selections.txt!");
                }
            }
            buttonResume.Visible = false;
        }

        private void ResetQueues()
        {
            resumeSelections = new Queue<int>(); // loaded from file (when resume button pressed)
            saveSelections = new Queue<int>(); // saved to file (when hits bottom of species tree)
        }

        // generate a random number from 0 to max, inclusive that is [0, max]
        private int RandomNumber(int max)
        {
            Random rng = new Random();
            return rng.Next(0, max + 1);
        }

        private int SelectLessonNum(dynamic katas) {
            int numLessons = katas.Lessons.Content.Count;
            return RandomNumber(numLessons - 1);
        }

        private int SelectExerciseNum(dynamic lesson) {
            int numExercises = lesson.Exercises.Count;
            int exerciseNum = RandomNumber(numExercises - 1);
            return exerciseNum;
        }

        private dynamic SelectEverydayObject(int exerciseNum) {
            string everydayObjectsFile = "Configuration\\JSON\\EverydayObjects.json";
            dynamic everydayObjects = LoadJson(everydayObjectsFile);

            dynamic everydayObjectGroup = everydayObjects["Everyday Objects"][exerciseNum];
            int exerciseCount = everydayObjectGroup.Content.Count;

            int subExerciseNum = RandomNumber(exerciseCount - 1);
            return everydayObjectGroup.Content[subExerciseNum]; 
        }

        private dynamic SelectAnimal(dynamic kingdom, dynamic exercise, int exerciseNum)
        {
            return null;
        }

        private dynamic SelectInsect(dynamic kingdom, dynamic exercise, int exerciseNum)
        {
            return null;
        }


        private int GetTaxonNum(dynamic taxonomicRank)
        {
            int taxonNum = 0;
            if (resuming) {
                taxonNum = resumeSelections.Dequeue();
            } else {
                taxonNum = RandomNumber(taxonomicRank.Content.Count - 1);
            }
            saveSelections.Enqueue(taxonNum);
            return taxonNum;
        }

        private dynamic PickNextTaxon(dynamic currentTaxon, string taxonString)
        {
            dynamic taxonomicRank = currentTaxon.Content;
            if (taxonomicRank.Content != null) {
                int taxonNum = GetTaxonNum(taxonomicRank);
                dynamic taxon = taxonomicRank.Content[taxonNum];

                if (taxonomicRank.Taxonomic_Rank == "Species") {
                    // we're done!
                    return taxon;
                } else {
                    MessageBox.Show("Partial taxonomy selected for species sub-exercies, please enter more information to continue and press \"Resume Selection\" to continue.");
                    return DescendTaxon(taxon, taxonString);
                }
            } else {
                throw new Exception("Error picking next taxon: Taxonmic Rank has no Content!");
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
                MessageBox.Show("Could not descend taxon: got error " + e);
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
            if (resuming) {
                selectedKingdomIndex = resumeSelections.Dequeue();
                selectedKingdom = kingdoms[selectedKingdomIndex];
            } else {
                int i = 0;
                foreach (dynamic kingdom in kingdoms) {
                    if (kingdom.Taxon_Name == "Plantae" &&
                        lessonNum == (byte)KataLessons.Plants
                    ) {
                        selectedKingdom = kingdom;
                        selectedKingdomIndex = i;
                    } else if (kingdom.Taxon_Name == "Animalia" &&
                        (
                            lessonNum == (byte)KataLessons.Animals ||
                            lessonNum == (byte)KataLessons.Insects
                        )   
                    ) {
                        selectedKingdom = kingdom;
                        selectedKingdomIndex = i;
                    }
                    i++;
                }

            }
            saveSelections.Enqueue(selectedKingdomIndex);
            return selectedKingdom;
        }

        private dynamic SelectSpecies(dynamic exercise, int lessonNum, int exerciseNum)
        {
            string speciesFile = "Configuration\\JSON\\Species.json";
            dynamic species = LoadJson(speciesFile);
            dynamic kingdom = GetKingdom(species.Kingdoms, lessonNum);

            switch(lessonNum) {
            case (byte)KataLessons.Plants:
                return SelectPlant(kingdom, exercise, exerciseNum);
            case (byte)KataLessons.Insects:
                return SelectInsect(kingdom, exercise, exerciseNum);
            case (byte)KataLessons.Animals:
                return SelectAnimal(kingdom, exercise, exerciseNum);
            default:
                MessageBox.Show("Error, incorrect lesson!");
                return "";
            }
        }
        
        private int GetLessonNum(dynamic katas)
        {
            int lessonNum = 0;
            if (comboBoxLesson.SelectedIndex >= 0) { // handles selecting from only one lesson, for now
                lessonNum = comboBoxLesson.SelectedIndex;        
            } else {
                if (resuming) {
                   lessonNum = resumeSelections.Dequeue();
                } else {
                    lessonNum = SelectLessonNum(katas);
                }
            }
            saveSelections.Enqueue(lessonNum);
            return lessonNum;
        }

        private int GetExerciseNum(dynamic lesson)
        {
            int exerciseNum = 0;
            if (resuming) {
                exerciseNum = resumeSelections.Dequeue();
            } else {
                exerciseNum = SelectExerciseNum(lesson);
            }
            saveSelections.Enqueue(exerciseNum);
            return exerciseNum;
        }

        private void pickDrawaboxExerciseRandomly() {
            string kataFile = "Configuration\\JSON\\DrawaboxKatas.json";
            dynamic katas = LoadJson(kataFile);

            int lessonNum = GetLessonNum(katas);
            dynamic lesson = null;
            lesson = katas.Lessons.Content[lessonNum]; 

            int exerciseNum = GetExerciseNum(lesson);
            dynamic exercise = lesson.Exercises[exerciseNum];
            dynamic subExercise = null;
            
            if (lessonNum == (byte)KataLessons.Objects) {
                subExercise = SelectEverydayObject(exerciseNum);  
            } else if (lessonNum == (byte)KataLessons.Animals ||
                    lessonNum == (byte)KataLessons.Insects ||
                    (lessonNum == (byte)KataLessons.Plants && exerciseNum == 2) //Kingdom Plantae exercise
                    ) {
                subExercise = SelectSpecies(exercise, lessonNum, exerciseNum);
            }

            string messageExercise = string.Format("Lesson: {0}\nExercise: {1}", lesson.Name, exercise);
            string messageSubexercise = "";

            if (subExercise != null) {
                messageSubexercise = string.Format("\nSubexercise: {0}", subExercise);
            }

            string message = messageExercise + messageSubexercise;

            labelResult.Text = message;
        }

        private void pickMusicExerciseRandomly() {
            string musicKataFile = "Configuration\\JSON\\Music.json";
            dynamic musicKatas = LoadJson(musicKataFile);

            int bpmIndex = RandomNumber(musicKatas.BPM.Count - 1);
            int bpm = musicKatas.BPM[bpmIndex];

            int keyIndex = RandomNumber(musicKatas.Key.Count - 1);
            string key = musicKatas.Key[keyIndex];

            string message = string.Format("BPM - {0}\nKey - {1}", bpm, key);

            labelResult.Text = message;
        }

        private void buttonRandomSelect_Click(object sender, EventArgs e) {
            switch (comboBoxKataType.SelectedIndex) {
            case (byte)KataTypes.Drawabox:
                pickDrawaboxExerciseRandomly();
                break;
            case (byte)KataTypes.Music:
                pickMusicExerciseRandomly();
                break;
            default:
                labelResult.Text = "Cannot select Kata: No Kata Type selected yet. Please select a Kata Type from the dropdown above.";
                break;
                
            }
        }

        private void button3_Click(object sender, EventArgs e) {

        }

        private void labelResult_Click(object sender, EventArgs e) {

        }

        private void comboBoxKataType_SelectedIndexChanged(object sender, EventArgs e) {
            if (comboBoxKataType.SelectedIndex == (byte)KataTypes.Music) {
                comboBoxLesson.Visible = false;
            } else {
                comboBoxLesson.Visible = true;
                PopulateList("Configuration\\JSON\\DrawaboxKatas.json");
            }

        }
    
        private void ResetResultText()
        {
            labelResult.ResetText();
            labelResult.Text = "Selected Kata";
        }
        private void Reset()
        {
            comboBoxKataType.ResetText();
            comboBoxKataType.SelectedIndex = -1;
            comboBoxKataType.Text = "Kata Type";

            comboBoxLesson.ResetText();
            comboBoxLesson.SelectedIndex = -1;
            comboBoxLesson.Text = "Only Choose From Specific Lesson";

            ResetResultText();
        }

        private void button1_Click_1(object sender, EventArgs e) {
            Reset();
        }

        private void buttonResume_Click(object sender, EventArgs e)
        {
            ResetResultText();
            ReadSelections();
            pickDrawaboxExerciseRandomly();
        }
    }
}
