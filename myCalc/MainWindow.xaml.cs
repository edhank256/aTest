using System;
using System.Collections.Generic;
using System.Linq;
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

namespace myCalc
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        char lastBinaryOp;      
        double total;
        bool shiftDown = false;
        public enum State
        {
            ZERO_FIRST,  //第一個運算元為零的狀態
            ZERO_INTERMED,  //第二個運算元為零的狀態
            OPERAND_FIRST_NODEC,  //第一個運算元無小數點的狀態
            OPERAND_FIRST_DEC,  //第一個運算元有小數點的狀態
            OPERAND_INTERMED_NODEC, //第二個運算元無小數點的狀態
            OPERAND_INTERMED_DEC, //第二個運算元有小數點的狀態
            AFTER_EQUALS, //輸入等號後的狀態
            OPERATOR  //處理運算子的狀態
        }
        State state;

        public MainWindow()
        {
            InitializeComponent();
            initializeState();
        }

        //初始設定
        private void initializeState()
        {
            state = State.ZERO_FIRST;  //初始狀態:第一個運算元為零的狀態
            total = 0.0;  //初始計算值(實數)
            tbDisplay.Text = "0.";  //預設顯示「0.」
        }

        //加分項：退位鍵處理(5%)
        private void handleBackspace()
        {
            
        }

        //處理運算子的輸入：將運算元數值存下或進行運算
        private void handleBinaryOp(char op)
        {
            switch (state)
            {
                case State.OPERAND_FIRST_NODEC:
                case State.OPERAND_FIRST_DEC:
                case State.AFTER_EQUALS:
                    state = State.OPERATOR;
                    total = double.Parse(tbDisplay.Text);
                    break;

                case State.OPERAND_INTERMED_NODEC:
                case State.OPERAND_INTERMED_DEC:
                    state = State.OPERATOR;
                    updateTotal();
                    break;
            }
            lastBinaryOp = op;
        }

        //處理點按清除鈕
        private void handleClear()
        {
            initializeState();
        }

        //處理小數點的輸入
        private void handleDecimal()
        {
            switch (state)
            {
                case State.ZERO_FIRST:
                    state = State.OPERAND_FIRST_DEC;
                    tbDisplay.Text = "0.";
                    break;

                case State.OPERAND_FIRST_NODEC:
                    state = State.OPERAND_FIRST_DEC;
                    break;

                case State.OPERAND_INTERMED_NODEC:
                    state = State.OPERAND_INTERMED_DEC;
                    break;

                case State.AFTER_EQUALS:
                    state = State.OPERAND_FIRST_DEC;
                    tbDisplay.Text = "0.";
                    break;

                case State.OPERATOR:
                    state = State.OPERAND_INTERMED_DEC;
                    tbDisplay.Text = "0.";
                    break;
            }
        }

        //處理數字的輸入
        private void handleDigit(int i)
        {
            switch (state)
            {
                case State.ZERO_FIRST:
                    if (i > 0)
                    {
                        tbDisplay.Text = i.ToString() + ".";
                        state = State.OPERAND_FIRST_NODEC;
                    }
                    break;

                case State.OPERAND_FIRST_NODEC:
                    tbDisplay.Text = tbDisplay.Text.Substring(0, tbDisplay.Text.Length - 1) + i.ToString() + ".";
                    break;

                case State.OPERAND_FIRST_DEC:
                    tbDisplay.Text = tbDisplay.Text + i.ToString();
                    break;

                case State.OPERAND_INTERMED_NODEC:
                    tbDisplay.Text = tbDisplay.Text.Substring(0, tbDisplay.Text.Length - 1) + i.ToString() + ".";
                    break;

                case State.OPERAND_INTERMED_DEC:
                    tbDisplay.Text = tbDisplay.Text + i.ToString();
                    break;

                case State.AFTER_EQUALS:
                    if (i != 0)
                    {
                        state = State.OPERAND_FIRST_NODEC;
                        tbDisplay.Text = i.ToString() + ".";
                        break;
                    }
                    state = State.ZERO_FIRST;
                    tbDisplay.Text = "0.";
                    break;

                case State.OPERATOR:
                    if (i != 0)
                    {
                        state = State.OPERAND_INTERMED_NODEC;
                        tbDisplay.Text = i.ToString() + ".";
                        break;
                    }
                    state = State.ZERO_INTERMED;
                    tbDisplay.Text = "0.";
                    break;
            }
        }

        //處理等號的輸入
        private void handleEquals()
        {
            if (tbDisplay.Text == "101306009.")  //顯示自己本人的系級資訊(20%)
                MessageBox.Show("<101306009> <資管二甲> <林睿峰>");
            switch (state)
            {
                case State.OPERAND_INTERMED_NODEC:
                case State.OPERAND_INTERMED_DEC:
                    updateTotal();
                    if (tbDisplay.Text[tbDisplay.Text.Length - 1] != '.')
                    {
                        state = State.OPERAND_FIRST_DEC;
                        break;
                    }
                    state = State.OPERAND_FIRST_NODEC;
                    break;

                default:
                    return;
            }
            state = State.AFTER_EQUALS;
        }

        //處理"+/-"改變符號的輸入
        private void handleNegate()
        {
            switch (state)
            {
                case State.OPERAND_FIRST_NODEC:
                case State.OPERAND_FIRST_DEC:
                case State.OPERAND_INTERMED_NODEC:
                case State.OPERAND_INTERMED_DEC:
                    if (tbDisplay.Text[0] != '-')
                    {
                        tbDisplay.Text = "-" + tbDisplay.Text;
                        break;
                    }
                    tbDisplay.Text = tbDisplay.Text.Substring(1);
                    break;
            }
        }

        //四則計算
        private void updateTotal()
        {
            double num = double.Parse(tbDisplay.Text);
            switch (lastBinaryOp)
            {
                case '*':
                    total *= num;
                    break;

                case '+':
                    total += num;
                    break;

                case '-':
                    total -= num;
                    break;

                case '/':
                    total /= num;
                    break;
            }
            tbDisplay.Text = total.ToString();
            if (tbDisplay.Text.IndexOf('.') == -1)  //如果計算結果沒有小數點，就在最後一位補上。
            {
                tbDisplay.Text = tbDisplay.Text + ".";
            }
        }

        private void Window_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.Source.GetType() == typeof(Button))
            {
                string sBC = ((Button)e.Source).Content.ToString();  //取出按鈕上的字串
                switch (sBC)
                {
                    case "0":
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8":
                    case "9":
                        handleDigit(Int32.Parse(sBC));
                        break;
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                        handleBinaryOp(sBC[0]);
                        break;
                    case ".":
                        handleDecimal();
                        break;
                    case "+/-":
                        handleNegate();
                        break;
                    case "C":
                        handleClear();
                        break;
                    case "=":
                        handleEquals();
                        break;
                    case "<-":
                        handleBackspace();
                        break;
                }                 
            }
        }

        //點選「結束(_X)」選單項目後結束程式
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //加分項：鍵盤熱鍵輸入(5%)
        //所使用的通道事件處理常式
        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            Key keyDown = e.Key;
            switch (keyDown) { 
                case Key.D1:
                case Key.NumPad1:
                    handleDigit(1);
                break;
                case Key.D2:
                case Key.NumPad2:
                    handleDigit(2);
                break;
                case Key.D3:
                case Key.NumPad3:
                    handleDigit(3);
                    
                break;
                case Key.Multiply:
                    handleBinaryOp('*');
                break;
                case Key.Add:
                    handleBinaryOp('+');
                break;
                case Key.Divide:
                    handleBinaryOp('/');
                break;
                case Key.Subtract:
                    handleBinaryOp('-');
                break;

        }
           
            
        }

        //搭配加分項的通道事件處理常式
        private void Window_PreviewKeyUp_1(object sender, KeyEventArgs e)
        {
            
        }

        //加分項：關閉視窗確認詢問(5%) 
        //所使用的事件處理常式
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void bt1_Click(object sender, RoutedEventArgs e)
        {
      
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.Show();
        }

    
       

      

       

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (MessageBox.Show("This is Content",
                                   "This is Title", MessageBoxButton.OKCancel,
                                   MessageBoxImage.Question) == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            };
        }

       



    }
}
