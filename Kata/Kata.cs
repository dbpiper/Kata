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
        
        private void pickDrawaboxExerciseRandomly() {
            string kataFile = "Configuration\\JSON\\DrawaboxKatas.json";
            dynamic katas = LoadJson(kataFile);

            int lessonNum = SelectLessonNum(katas);
            dynamic lesson = null;

            if (comboBoxLesson.SelectedIndex >= 0) {
                lessonNum = comboBoxLesson.SelectedIndex;        
            }

            lesson = katas.Lessons.Content[lessonNum]; 

            int exerciseNum = SelectExerciseNum(lesson); 
            dynamic exercise = lesson.Exercises[exerciseNum];
            dynamic subExercise = null;
            
            if (lessonNum == 5) {  // everyday object
                subExercise = SelectEverydayObject(exerciseNum);  
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

        private void button1_Click_1(object sender, EventArgs e) {
            comboBoxKataType.ResetText();
            comboBoxKataType.SelectedIndex = -1;
            comboBoxKataType.Text = "Kata Type";

            comboBoxLesson.ResetText();
            comboBoxLesson.SelectedIndex = -1;
            comboBoxLesson.Text = "Only Choose From Specific Lesson";

            labelResult.ResetText();
            labelResult.Text = "Selected Kata";
        }
    }
}
