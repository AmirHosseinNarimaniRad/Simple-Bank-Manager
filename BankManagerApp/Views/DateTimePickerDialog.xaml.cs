using System.Globalization;

namespace BankManagerApp.Views
{
    public partial class DateTimePickerDialog : ContentPage
    {
        private readonly PersianCalendar _persianCalendar = new PersianCalendar();
        private readonly string[] _monthNames = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", 
                                                  "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
        
        private readonly TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();
        
        public DateTime? SelectedDateTime { get; private set; }

        public DateTimePickerDialog(DateTime? initialValue = null)
        {
            InitializeComponent();
            
            // Initialize pickers
            InitializePickers(initialValue ?? DateTime.Now);
        }

        public Task WaitForResultAsync()
        {
            return _taskCompletionSource.Task;
        }

        private void InitializePickers(DateTime dateTime)
        {
            // Year picker (1380-1410)
            for (int year = 1380; year <= 1410; year++)
            {
                YearPicker.Items.Add(year.ToString());
            }

            // Month picker
            foreach (var month in _monthNames)
            {
                MonthPicker.Items.Add(month);
            }

            // Set current values
            int pYear = _persianCalendar.GetYear(dateTime);
            int pMonth = _persianCalendar.GetMonth(dateTime);
            int pDay = _persianCalendar.GetDayOfMonth(dateTime);

            YearPicker.SelectedIndex = pYear - 1380;
            MonthPicker.SelectedIndex = pMonth - 1;
            
            UpdateDayPicker();
            
            // Set day after updating day picker
            if (pDay <= DayPicker.Items.Count)
            {
                DayPicker.SelectedIndex = pDay - 1;
            }

            // Set time
            HourEntry.Text = dateTime.Hour.ToString("00");
            MinuteEntry.Text = dateTime.Minute.ToString("00");
        }

        private void OnDateChanged(object sender, EventArgs e)
        {
            UpdateDayPicker();
        }

        private void UpdateDayPicker()
        {
            if (YearPicker.SelectedIndex < 0 || MonthPicker.SelectedIndex < 0)
                return;

            int selectedYear = 1380 + YearPicker.SelectedIndex;
            int selectedMonth = MonthPicker.SelectedIndex + 1;
            
            // Get number of days in selected month
            int daysInMonth = _persianCalendar.GetDaysInMonth(selectedYear, selectedMonth);
            
            int currentSelection = DayPicker.SelectedIndex;
            
            DayPicker.Items.Clear();
            for (int day = 1; day <= daysInMonth; day++)
            {
                DayPicker.Items.Add(day.ToString());
            }
            
            // Restore selection or set to last day if previous selection was out of range
            if (currentSelection >= 0 && currentSelection < daysInMonth)
            {
                DayPicker.SelectedIndex = currentSelection;
            }
            else if (currentSelection >= daysInMonth)
            {
                DayPicker.SelectedIndex = daysInMonth - 1;
            }
            else
            {
                DayPicker.SelectedIndex = 0;
            }
        }

        private void OnNowClicked(object sender, EventArgs e)
        {
            InitializePickers(DateTime.Now);
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            SelectedDateTime = null;
            _taskCompletionSource.SetResult(true);
            await Navigation.PopModalAsync();
        }

        private async void OnConfirmClicked(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (YearPicker.SelectedIndex < 0 || MonthPicker.SelectedIndex < 0 || DayPicker.SelectedIndex < 0)
                {
                    await DisplayAlert("خطا", "لطفاً تاریخ را کامل وارد کنید", "باشه");
                    return;
                }

                if (!int.TryParse(HourEntry.Text, out int hour) || hour < 0 || hour > 23)
                {
                    await DisplayAlert("خطا", "ساعت باید بین 0 تا 23 باشد", "باشه");
                    return;
                }

                if (!int.TryParse(MinuteEntry.Text, out int minute) || minute < 0 || minute > 59)
                {
                    await DisplayAlert("خطا", "دقیقه باید بین 0 تا 59 باشد", "باشه");
                    return;
                }

                // Convert Persian date to Gregorian
                int pYear = 1380 + YearPicker.SelectedIndex;
                int pMonth = MonthPicker.SelectedIndex + 1;
                int pDay = DayPicker.SelectedIndex + 1;

                SelectedDateTime = _persianCalendar.ToDateTime(pYear, pMonth, pDay, hour, minute, 0, 0);
                
                _taskCompletionSource.SetResult(true);
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("خطا", $"تاریخ نامعتبر: {ex.Message}", "باشه");
            }
        }
    }
}
