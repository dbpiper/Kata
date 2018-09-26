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
            PopulateList("Configuration\\JSON\\DrawaboxKatas.json");
        }

        private void comboBoxLesson_SelectedIndexChanged(object sender, EventArgs e) {

        }
           
        private int RandomNumber(int max)
        {
            Random rng = new Random();
            return rng.Next(0, max);
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

        private void buttonRandomSelect_Click(object sender, EventArgs e) {
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
    }
}
