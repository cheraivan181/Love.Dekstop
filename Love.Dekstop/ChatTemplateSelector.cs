using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Love.Dekstop
{
    public class ChatTemplateSelector : DataTemplateSelector
    {

        public ChatTemplateSelector()
        {
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {          
            var messageVm = item as TestMessage;
            FrameworkElement elemnt = container as FrameworkElement;

            if (messageVm.Sender == ContactsForm.Iam)
                return elemnt.FindResource("Outgoing") as DataTemplate;
            else
                return elemnt.FindResource("Incoming") as DataTemplate;
        }
    }
}
