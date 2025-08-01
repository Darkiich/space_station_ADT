using Content.Shared.Chat;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using static Robust.Client.UserInterface.Controls.BaseButton;

namespace Content.Client.UserInterface.Systems.Chat.Controls;

[GenerateTypedNameReferences]
public sealed partial class ChannelFilterPopup : Popup
{
    // order in which the available channel filters show up when available
    private static readonly ChatChannel[] ChannelFilterOrder =
    {
        ChatChannel.Local,
        ChatChannel.Whisper,
        ChatChannel.Emotes,
        ChatChannel.Radio,
        ChatChannel.CollectiveMind, // ADT-CollectiveMind-Tweak
        ChatChannel.Notifications,
        ChatChannel.LOOC,
        ChatChannel.OOC,
        ChatChannel.Dead,
        ChatChannel.Admin,
        ChatChannel.AdminAlert,
        // ChatChannel.AdminChat, // ADT-Tweak: Убираем фильтр а-чата. (Должен быть всегда включен)
        ChatChannel.Server
    };

    private readonly Dictionary<ChatChannel, ChannelFilterCheckbox> _filterStates = new();

    public event Action<ChatChannel, bool>? OnChannelFilter;

    public ChannelFilterPopup()
    {
        RobustXamlLoader.Load(this);
    }

    public bool IsActive(ChatChannel channel)
    {
        // ADT-Tweak-start
        // AdminChat всегда активен
        if (channel == ChatChannel.AdminChat)
        {
            return true;
        }
        // ADT-Tweak-end
        return _filterStates.TryGetValue(channel, out var checkbox) && checkbox.Pressed;
    }

    public void SetActive(ChatChannel channel, bool isActive)
    {
        if (_filterStates.TryGetValue(channel, out var checkbox) && checkbox.Pressed != isActive)
        {
            checkbox.Pressed = isActive;
            OnChannelFilter?.Invoke(checkbox.Channel, checkbox.Pressed);
        }
    }

    public ChatChannel GetActive()
    {
        ChatChannel active = 0;

        foreach (var (key, value) in _filterStates)
        {
            if (value.IsHidden || !value.Pressed)
            {
                continue;
            }

            active |= key;
        }

        return active;
    }

    public void SetChannels(ChatChannel channels)
    {
        foreach (var channel in ChannelFilterOrder)
        {
            if (!_filterStates.TryGetValue(channel, out var checkbox))
            {
                checkbox = new ChannelFilterCheckbox(channel);
                _filterStates.Add(channel, checkbox);
                checkbox.OnPressed += CheckboxPressed;
                checkbox.Pressed = true;
            }

            if ((channels & channel) == 0)
            {
                if (checkbox.Parent == FilterVBox)
                {
                    FilterVBox.RemoveChild(checkbox);
                }
            }
            else if (checkbox.IsHidden)
            {
                FilterVBox.AddChild(checkbox);
            }
        }
    }

    private void CheckboxPressed(ButtonEventArgs args)
    {
        var checkbox = (ChannelFilterCheckbox) args.Button;
        OnChannelFilter?.Invoke(checkbox.Channel, checkbox.Pressed);
    }

    public void UpdateUnread(ChatChannel channel, int? unread)
    {
        if (_filterStates.TryGetValue(channel, out var checkbox))
            checkbox.UpdateUnreadCount(unread);
    }
}
