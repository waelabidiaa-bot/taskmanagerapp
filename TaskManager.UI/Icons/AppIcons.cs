using Avalonia.Media;

namespace TaskManager.UI.Icons;

// Small inline vector icons (Material-style paths). No external icon package needed.
public static class AppIcons
{
    public static StreamGeometry Dashboard { get; } = StreamGeometry.Parse(
        "M3 13h8V3H3v10zm0 8h8v-6H3v6zm10 0h8V11h-8v10zm0-18v6h8V3h-8z");

    public static StreamGeometry AllTasks { get; } = StreamGeometry.Parse(
        "M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-2 6H9v2h8V9zm0 4H9v2h8v-2zm0 4H9v2h8v-2zM7 9H5v2h2V9zm0 4H5v2h2v-2zm0 4H5v2h2v-2z");

    public static StreamGeometry Tasks { get; } = StreamGeometry.Parse(
        "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z");

    public static StreamGeometry Notes { get; } = StreamGeometry.Parse(
        "M14 2H6c-1.1 0-1.99.9-1.99 2L4 20c0 1.1.89 2 1.99 2H18c1.1 0 2-.9 2-2V8l-6-6zm2 16H8v-2h8v2zm0-4H8v-2h8v2zm-3-5V3.5L18.5 9H13z");

    public static StreamGeometry Learning { get; } = StreamGeometry.Parse(
        "M12 3 1 9l11 6 9-4.91V17h2V9L12 3zm-7 10.18V15l7 3.82 7-3.82v-1.82L12 17l-7-3.82z");

    public static StreamGeometry Categories { get; } = StreamGeometry.Parse(
        "M10 4H4c-1.1 0-1.99.9-1.99 2L2 18c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V8c0-1.1-.9-2-2-2h-8l-2-2z");

    public static StreamGeometry Settings { get; } = StreamGeometry.Parse(
        "M19.14 12.94c.04-.3.06-.61.06-.94 0-.32-.02-.64-.07-.94l2.03-1.58a.5.5 0 0 0 .12-.64l-1.92-3.32a.5.5 0 0 0-.61-.22l-2.39.96a7.03 7.03 0 0 0-1.62-.94l-.36-2.54a.5.5 0 0 0-.5-.42h-3.84a.5.5 0 0 0-.5.42l-.36 2.54c-.59.24-1.13.56-1.62.94l-2.39-.96a.5.5 0 0 0-.61.22L2.71 8.84a.5.5 0 0 0 .12.64l2.03 1.58c-.05.3-.09.63-.09.94s.02.64.07.94l-2.03 1.58a.5.5 0 0 0-.12.64l1.92 3.32c.15.26.46.36.73.22l2.39-.96c.5.38 1.03.7 1.62.94l.36 2.54c.05.24.25.42.5.42h3.84c.25 0 .45-.18.5-.42l.36-2.54c.59-.24 1.13-.56 1.62-.94l2.39.96c.27.14.58.04.73-.22l1.92-3.32a.5.5 0 0 0-.12-.64l-2.02-1.58zM12 15.6a3.6 3.6 0 1 1 0-7.2 3.6 3.6 0 0 1 0 7.2z");

    public static StreamGeometry Add { get; } = StreamGeometry.Parse(
        "M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z");

    public static StreamGeometry Save { get; } = StreamGeometry.Parse(
        "M17 3H5a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V7l-4-4zm-5 16a3 3 0 1 1 0-6 3 3 0 0 1 0 6zm3-10H5V5h10v4z");

    public static StreamGeometry Clear { get; } = StreamGeometry.Parse(
        "M19 6.41 17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12 19 6.41z");

    public static StreamGeometry Delete { get; } = StreamGeometry.Parse(
        "M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6v12zM19 4h-3.5l-1-1h-5l-1 1H5v2h14V4z");

    public static StreamGeometry Edit { get; } = StreamGeometry.Parse(
        "M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zM20.71 7.04a.996.996 0 0 0 0-1.41l-2.34-2.34a.996.996 0 0 0-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z");

    public static StreamGeometry Check { get; } = StreamGeometry.Parse(
        "M9 16.17 4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z");

    public static StreamGeometry Refresh { get; } = StreamGeometry.Parse(
        "M17.65 6.35A8 8 0 1 0 19.73 15h-2.08a6 6 0 1 1-1.38-6.66L14 10h6V4l-2.35 2.35z");

    public static StreamGeometry Pin { get; } = StreamGeometry.Parse(
        "M16 3v2h-2v6l3 3v2h-6v6h-2v-6H6v-2l3-3V5H7V3h9z");

    public static StreamGeometry Milestone { get; } = StreamGeometry.Parse(
        "M14.4 6 14 4H5v17h2v-7h5.6l.4 2h7V6h-5.6z");
}
