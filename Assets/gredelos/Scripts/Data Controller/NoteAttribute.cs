using UnityEngine;

public class NoteAttribute : PropertyAttribute
{
    public string text;

    public NoteAttribute(string text)
    {
        this.text = text;
    }
}
