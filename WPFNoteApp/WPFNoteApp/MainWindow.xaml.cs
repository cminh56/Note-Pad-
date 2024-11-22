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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Tạo ghi chú từ giao diện
            var note = new Note
            {
                Title = TitleTextBox.Text,
                Content = new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd).Text
            };

            // Hộp thoại lưu file
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                // Kiểm tra định dạng file
                if (filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    // Lưu dưới dạng JSON
                    string json = JsonSerializer.Serialize(note, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, json);
                    MessageBox.Show("Ghi chú đã được lưu dưới dạng JSON!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    // Lưu dưới dạng TXT
                    string txtContent = $"Tiêu đề: {note.Title}\n\n{note.Content}";
                    File.WriteAllText(filePath, txtContent);
                    MessageBox.Show("Ghi chú đã được lưu dưới dạng TXT!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Định dạng không được hỗ trợ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            // Hộp thoại mở file
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                if (filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    // Đọc và xử lý file JSON
                    string json = File.ReadAllText(filePath);
                    var note = JsonSerializer.Deserialize<Note>(json);

                    if (note != null)
                    {
                        TitleTextBox.Text = note.Title;

                        ContentRichTextBox.Document.Blocks.Clear();
                        ContentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(note.Content)));
                    }
                }
                else if (filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    // Đọc và xử lý file TXT
                    string txtContent = File.ReadAllText(filePath);

                    // Tách tiêu đề và nội dung (giả định tiêu đề nằm trên dòng đầu tiên)
                    string[] lines = txtContent.Split(new[] { '\n' }, 2); // Chia thành 2 phần
                    TitleTextBox.Text = lines.Length > 0 ? lines[0].Replace("Tiêu đề: ", "").Trim() : "";
                    string content = lines.Length > 1 ? lines[1].Trim() : "";

                    ContentRichTextBox.Document.Blocks.Clear();
                    ContentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(content)));
                }
                else
                {
                    MessageBox.Show("Định dạng file không được hỗ trợ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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