using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFNoteApp.Models;

namespace WPFNoteApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Xóa nội dung để tạo ghi chú mới
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            TitleTextBox.Text = string.Empty;
            ContentRichTextBox.Document.Blocks.Clear();
        }

        // Lưu ghi chú vào file JSON
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var note = new Note
            {
                Title = TitleTextBox.Text,
                Content = new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd).Text
            };

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string json = JsonSerializer.Serialize(note, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(saveFileDialog.FileName, json);
                MessageBox.Show("Ghi chú đã được lưu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Mở ghi chú từ file JSON
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                var note = JsonSerializer.Deserialize<Note>(json);

                if (note != null)
                {
                    TitleTextBox.Text = note.Title;

                    ContentRichTextBox.Document.Blocks.Clear();
                    ContentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(note.Content)));
                }
            }
        }

        // Highlight văn bản được chọn
        private void HighlightButton_Click(object sender, RoutedEventArgs e)
        {
            TextSelection selectedText = ContentRichTextBox.Selection;

            if (!selectedText.IsEmpty)
            {
                selectedText.ApplyPropertyValue(TextElement.BackgroundProperty, System.Windows.Media.Brushes.Yellow);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn văn bản để highlight!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

}