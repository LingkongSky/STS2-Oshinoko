using Godot;

public partial class OshinogoBackground : Control
{
	[Export] public Vector2 BaseSize { get; set; } = new(1920, 1080);
	[Export] public NodePath StageRootPath { get; set; } = "StageRoot";

	private Node2D? _stageRoot;
	private Vector2I _lastViewportSize;

	public override void _Ready()
	{
		MouseFilter = MouseFilterEnum.Ignore;
		_stageRoot = GetNodeOrNull<Node2D>(StageRootPath);
		UpdateLayout(force: true);
		SetProcess(true);
	}

	public override void _Process(double delta) => UpdateLayout(force: false);

	private void UpdateLayout(bool force)
	{
		if (_stageRoot == null)
			return;

		var viewportSize = Size;
		if (viewportSize.X <= 0 || viewportSize.Y <= 0)
		{
			var viewport = GetViewport();
			if (viewport != null)
				viewportSize = viewport.GetVisibleRect().Size;
		}

		var viewportSizeI = new Vector2I((int)viewportSize.X, (int)viewportSize.Y);

		if (!force && viewportSizeI == _lastViewportSize)
			return;

		_lastViewportSize = viewportSizeI;

		if (BaseSize.X <= 0 || BaseSize.Y <= 0)
			return;

		// Scale to cover the viewport (crop if needed) and keep centered.
		float scale = Mathf.Max(viewportSize.X / BaseSize.X, viewportSize.Y / BaseSize.Y);
		_stageRoot.Scale = new Vector2(scale, scale);
		_stageRoot.Position = viewportSize * 0.5f;
	}
}
