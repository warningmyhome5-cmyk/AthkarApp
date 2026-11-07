using Plugin.Maui.Audio;

namespace AthkarApp;

public partial class MainPage : ContentPage
{
    private readonly IAudioManager audioManager;
    private bool isTimerRunning = false;
    private IDispatcherTimer reminderTimer;

    // تثبيت ذكر الصلاة على النبي ﷺ وملف الصوت الخاص به
    private readonly string salawatText = "اللَّهُمَّ صَلِّ وَسَلِّمْ عَلَى نَبِيِّنَا مُحَمَّدٍ";
    private readonly string salawatAudio = "salawat.mp3";

    public MainPage(IAudioManager audioManager)
    {
        InitializeComponent();
        this.audioManager = audioManager;

        // إنشاء المؤقت الداخلي
        reminderTimer = Dispatcher.CreateTimer();
        reminderTimer.Tick += (s, e) => PlaySalawatAudio();
    }

    // زر تجربة الصوت فوراً للتأكد من عمل الملف الصوتي
    private async void OnTestAudioClicked(object sender, EventArgs e)
    {
        await PlaySalawatAudio();
    }

    // تشغيل ملف الصوت مباشرة
    private async Task PlaySalawatAudio()
    {
        try
        {
            var stream = await FileSystem.OpenAppPackageFileAsync(salawatAudio);
            var player = audioManager.CreatePlayer(stream);
            player.Play();
        }
        catch (Exception ex)
        {
            await DisplayAlert("تنبيه", "تأكد من وجود ملف salawat.mp3 في مجلد Resources/Raw", "إغلاق");
        }
    }

    // تفعيل أو إيقاف المؤقت الداخلي بناءً على الدقائق المحددة
    private async void OnToggleReminderClicked(object sender, EventArgs e)
    {
        if (!isTimerRunning)
        {
            // التحقق من صحة الرقم المدخل للدقائق
            if (string.IsNullOrWhiteSpace(MinutesEntry.Text) || !int.TryParse(MinutesEntry.Text, out int minutes) || minutes <= 0)
            {
                await DisplayAlert("تنبيه", "الرجاء إدخال عدد دقائق صحيح (مثلاً 1، 2، 5)", "حسناً");
                return;
            }

            isTimerRunning = true;
            ToggleBtn.Text = "إيقاف التذكير المتكرر";
            ToggleBtn.BackgroundColor = Color.FromArgb("#8B0000");
            MinutesEntry.IsEnabled = false;

            // ضبط الوقت للمؤقت (التحويل من دقائق إلى تكة زمنية)
            reminderTimer.Interval = TimeSpan.FromMinutes(minutes);
            reminderTimer.Start();

            await DisplayAlert("تم التفعيل", $"سيتم تشغيل الصلاة على النبي ﷺ تلقائياً كل {minutes} دقائق.", "حسناً");
        }
        else
        {
            isTimerRunning = false;
            ToggleBtn.Text = "بدء التذكير المستمر";
            ToggleBtn.BackgroundColor = Color.FromArgb("#2D5A27");
            MinutesEntry.IsEnabled = true;

            // إيقاف المؤقت الداخلي
            reminderTimer.Stop();

            await DisplayAlert("تم الإيقاف", "تم إيقاف التذكير المتكرر بنجاح.", "حسناً");
        }
    }

    // عرض نافذة حول التطبيق
    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await DisplayAlert("حول التطبيق", "تطبيق تذكير بالصلاة على النبي ﷺ\n\nتطوير وبرمجة: إبراهيم غنيم\nبمساعدة: Gemini\n\nنسأل الله أن يجعله في ميزان حسناتنا جميعاً.", "إغلاق");
    }
}