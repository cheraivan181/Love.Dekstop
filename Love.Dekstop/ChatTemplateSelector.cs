using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Love.Dekstop
{
    public class ChatTemplateSelector : DataTemplateSelector
    {
        DataTemplate incomingDataTemplate;
        DataTemplate outgoingDataTemplate;

        public ChatTemplateSelector()
        {
            this.incomingDataTemplate = new DataTemplate(typeof(IncomingTemplate));
            this.outgoingDataTemplate = new DataTemplate(typeof(OutgoingTemplate));
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
          
            var messageVm = item as TestMessage;
            if (messageVm == null)
                return null;

            if (messageVm.Sender == ContactsForm.Iam)
                return outgoingDataTemplate;
            else
                return incomingDataTemplate;
        }
    }
}
