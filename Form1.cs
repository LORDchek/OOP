using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OOP.Shapes;

namespace OOP
{
    /// <summary> 
    /// Main form of the graphical editor application
    /// Demonstrates shape creation, static initialization, and interactive editing
    /// </summary>
    public class MainForm : Form
    {
        private ShapeList shapes = new ShapeList();
        private ShapeCreator shapeCreator = new ShapeCreator();
        private ComboBox shapeTypeCombo;
        private Button colorButton;
        private Panel drawingPanel;
        private ListBox shapesListBox;
        private Button deleteButton;
        private Button clearButton;
        private NumericUpDown xInput;
        private NumericUpDown yInput;
        private NumericUpDown widthInput;
        private NumericUpDown heightInput;
        private Button applyChangesButton;
        private Button saveXmlButton;
        private Button loadXmlButton;
        private ColorDialog colorDialog = new ColorDialog();
        private readonly ShapeRendererRegistry rendererRegistry = new ShapeRendererRegistry();
        private readonly ShapeSerializerRegistry serializerRegistry = new ShapeSerializerRegistry();
        private readonly DataProcessingPipeline dataProcessingPipeline = new DataProcessingPipeline();
        private readonly PluginLoader pluginLoader = new PluginLoader();
        private ShapeDrawer shapeDrawer;
        private ShapeXmlService xmlService;
        private Label pluginStatusLabel;
        private MenuStrip menuStrip;
        private ToolStripMenuItem dataProcessingMenu;
        private PluginLoadResult lastPluginLoadResult = new PluginLoadResult();

        public MainForm(string[] pluginArguments = null)
        {
            ShapePluginInfrastructure.RegisterBuiltIn(rendererRegistry, serializerRegistry);
            shapeDrawer = new ShapeDrawer(rendererRegistry);
            xmlService = new ShapeXmlService(serializerRegistry, dataProcessingPipeline);
            LoadPlugins(pluginArguments ?? Array.Empty<string>(), fullReload: true);

            InitializeComponent();
            RefreshShapeTypeList();
            InitializeShapes(); // Static initialization
            UpdateShapesList();
        }

        /// <summary>
        /// Initializes form components
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "Graphical Editor";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeMainMenu();

            // Create toolbar panel
            Panel toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.LightGray
            };

            // Shape type combo box
            Label shapeLabel = new Label
            {
                Text = "Shape:",
                Location = new Point(10, 10),
                Size = new Size(50, 20)
            };

            shapeTypeCombo = new ComboBox
            {
                Location = new Point(70, 8),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            shapeTypeCombo.SelectedIndexChanged += (s, e) =>
            {
                if (shapeTypeCombo.SelectedItem != null)
                {
                    shapeCreator.SetCurrentShape(shapeTypeCombo.SelectedItem.ToString());
                }
            };

            // Color button
            colorButton = new Button
            {
                Text = "Color",
                Location = new Point(200, 8),
                Size = new Size(60, 25),
                BackColor = Color.Black,
                ForeColor = Color.White
            };

            colorButton.Click += (s, e) =>
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    colorButton.BackColor = colorDialog.Color;
                    shapeCreator.SetCurrentColor(colorDialog.Color);
                }
            };

            // Add default shapes button
            Button addDefaultBtn = new Button
            {
                Text = "Add Default",
                Location = new Point(270, 8),
                Size = new Size(90, 25)
            };

            addDefaultBtn.Click += (s, e) =>
            {
                AddDefaultShapes();
                UpdateShapesList();
                drawingPanel.Invalidate();
            };

            saveXmlButton = new Button
            {
                Text = "Save XML",
                Location = new Point(370, 8),
                Size = new Size(80, 25)
            };
            saveXmlButton.Click += SaveXmlButton_Click;

            loadXmlButton = new Button
            {
                Text = "Load XML",
                Location = new Point(460, 8),
                Size = new Size(80, 25)
            };
            loadXmlButton.Click += LoadXmlButton_Click;

            pluginStatusLabel = new Label
            {
                Text = GetPluginStatusText(),
                Location = new Point(550, 10),
                Size = new Size(330, 20)
            };

            toolbar.Controls.AddRange(new Control[] {
                shapeLabel, shapeTypeCombo, colorButton, addDefaultBtn, saveXmlButton, loadXmlButton, pluginStatusLabel
            });

            // Create drawing panel
            drawingPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            drawingPanel.Paint += DrawingPanel_Paint;
            drawingPanel.MouseDown += DrawingPanel_MouseDown;
            drawingPanel.MouseUp += DrawingPanel_MouseUp;

            // Create right panel for shape list
            Panel rightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 200,
                BackColor = Color.LightGray
            };

            Label listLabel = new Label
            {
                Text = "Shapes:",
                Location = new Point(10, 10),
                Size = new Size(180, 20)
            };

            shapesListBox = new ListBox
            {
                Location = new Point(10, 35),
                Size = new Size(180, 250)
            };

            shapesListBox.SelectedIndexChanged += (s, e) =>
            {
                if (shapesListBox.SelectedIndex >= 0)
                {
                    // Highlight selected shape
                    for (int i = 0; i < shapes.Count; i++)
                    {
                        shapes[i].IsSelected = (i == shapesListBox.SelectedIndex);
                    }
                    SyncEditInputsFromSelectedShape();
                    drawingPanel.Invalidate();
                }
            };

            Label editLabel = new Label
            {
                Text = "Edit selected:",
                Location = new Point(10, 285),
                Size = new Size(180, 20)
            };

            rightPanel.Controls.Add(CreateInputLabel("X", new Point(10, 308)));
            rightPanel.Controls.Add(CreateInputLabel("Y", new Point(100, 308)));
            rightPanel.Controls.Add(CreateInputLabel("W", new Point(10, 343)));
            rightPanel.Controls.Add(CreateInputLabel("H", new Point(100, 343)));

            xInput = CreateNumericInput(new Point(10, 325));
            yInput = CreateNumericInput(new Point(100, 325));
            widthInput = CreateNumericInput(new Point(10, 360));
            heightInput = CreateNumericInput(new Point(100, 360));

            applyChangesButton = new Button
            {
                Text = "Apply changes",
                Location = new Point(10, 390),
                Size = new Size(180, 30)
            };
            applyChangesButton.Click += ApplyChangesButton_Click;

            deleteButton = new Button
            {
                Text = "Delete Selected",
                Location = new Point(10, 430),
                Size = new Size(180, 30)
            };

            deleteButton.Click += (s, e) =>
            {
                if (shapesListBox.SelectedIndex >= 0)
                {
                    shapes.RemoveAt(shapesListBox.SelectedIndex);
                    UpdateShapesList();
                    drawingPanel.Invalidate();
                }
            };

            clearButton = new Button
            {
                Text = "Clear All",
                Location = new Point(10, 465),
                Size = new Size(180, 30)
            };

            clearButton.Click += (s, e) =>
            {
                shapes.Clear();
                UpdateShapesList();
                drawingPanel.Invalidate();
            };

            rightPanel.Controls.AddRange(new Control[] {
                listLabel, shapesListBox, editLabel, xInput, yInput, widthInput, heightInput,
                applyChangesButton, deleteButton, clearButton
            });

            // Subscribe to shape creation event
            shapeCreator.ShapeCreated += ShapeCreator_ShapeCreated;

            // Add controls to form
            this.Controls.Add(drawingPanel);
            this.Controls.Add(rightPanel);
            this.Controls.Add(toolbar);
        }

        /// <summary>
        /// Static initialization of shapes
        /// </summary>
        private void InitializeShapes()
        {
            // Create various shapes at different positions
            shapes.AddShape(new Line(new Point(50, 50), new Point(150, 150), Color.Black));
            shapes.AddShape(new RectangleShape(new Point(200, 50), 100, 80, Color.Blue));
            shapes.AddShape(new Ellipse(new Point(350, 50), 120, 80, Color.Green));
            shapes.AddShape(new Triangle(
                new Point(50, 250),
                new Point(150, 250),
                new Point(100, 180),
                Color.Red));
            shapes.AddShape(new Square(new Point(200, 250), 70, Color.Orange));
            shapes.AddShape(new Circle(new Point(350, 290), 40, Color.Purple));
        }

        /// <summary>
        /// Adds default shapes using the factory
        /// </summary>
        private void AddDefaultShapes()
        {
            int x = 50;
            int y = 400;

            foreach (string shapeName in shapeCreator.GetAvailableShapes())
            {
                IShape shape = shapeCreator.CreateDefaultShape(shapeName, new Point(x, y));
                if (shape != null)
                {
                    shapes.AddShape(shape);
                    x += 100;
                    if (x > 600) { x = 50; y += 100; }
                }
            }
        }

        private void InitializeMainMenu()
        {
            menuStrip = new MenuStrip();

            var settingsMenu = new ToolStripMenuItem("Настройки");

            var pluginsMenu = new ToolStripMenuItem("Плагины");
            pluginsMenu.DropDownItems.Add("Загрузить плагин из файла...", null, LoadPluginFromFile_Click);
            pluginsMenu.DropDownItems.Add("Обновить из папки Plugins", null, ReloadPluginsFromFolder_Click);
            settingsMenu.DropDownItems.Add(pluginsMenu);

            settingsMenu.DropDownItems.Add(new ToolStripSeparator());

            dataProcessingMenu = new ToolStripMenuItem("Обработка данных");
            settingsMenu.DropDownItems.Add(dataProcessingMenu);

            menuStrip.Items.Add(settingsMenu);
            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);

            RebuildDataProcessingMenu();
        }

        private PluginHostContext CreatePluginHostContext()
        {
            return new PluginHostContext(shapeCreator, rendererRegistry, serializerRegistry, dataProcessingPipeline);
        }

        private void LoadPlugins(string[] pluginArguments, bool fullReload)
        {
            if (fullReload)
            {
                pluginLoader.ResetLoadedAssemblies();
                dataProcessingPipeline.Clear();
            }

            var context = CreatePluginHostContext();
            string pluginsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

            var fromFolder = pluginLoader.LoadFromFolder(pluginsFolder, context);
            var fromArgs = pluginLoader.LoadFromArguments(pluginArguments, context);

            lastPluginLoadResult = new PluginLoadResult
            {
                ShapePlugins = fromFolder.ShapePlugins + fromArgs.ShapePlugins,
                DataPlugins = fromFolder.DataPlugins + fromArgs.DataPlugins
            };

            RebuildDataProcessingMenu();
            UpdatePluginStatusLabel();
        }

        private void ReloadPluginsFromFolder_Click(object sender, EventArgs e)
        {
            LoadPlugins(Array.Empty<string>(), fullReload: true);
            RefreshShapeTypeList();
            drawingPanel.Invalidate();
            MessageBox.Show("Плагины перезагружены из папки Plugins.", "Плагины", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadPluginFromFile_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Plugin DLL (*.dll)|*.dll";
                dialog.Title = "Выберите файл плагина";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                var context = CreatePluginHostContext();
                var loaded = pluginLoader.LoadFromFile(dialog.FileName, context);

                lastPluginLoadResult.ShapePlugins += loaded.ShapePlugins;
                lastPluginLoadResult.DataPlugins += loaded.DataPlugins;

                RefreshShapeTypeList();
                RebuildDataProcessingMenu();
                UpdatePluginStatusLabel();
                drawingPanel.Invalidate();

                MessageBox.Show(
                    $"Загружено: фигур — {loaded.ShapePlugins}, обработки данных — {loaded.DataPlugins}.",
                    "Плагин",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void RebuildDataProcessingMenu()
        {
            if (dataProcessingMenu == null)
                return;

            dataProcessingMenu.DropDownItems.Clear();

            if (dataProcessingPipeline.Plugins.Count == 0)
            {
                dataProcessingMenu.DropDownItems.Add(new ToolStripMenuItem("(плагины обработки не загружены)") { Enabled = false });
                return;
            }

            foreach (var plugin in dataProcessingPipeline.Plugins)
            {
                var item = new ToolStripMenuItem(plugin.MenuText)
                {
                    CheckOnClick = true,
                    Checked = plugin.IsEnabled
                };

                item.CheckedChanged += (s, e) => plugin.IsEnabled = item.Checked;
                dataProcessingMenu.DropDownItems.Add(item);
            }
        }

        private string GetPluginStatusText()
        {
            return $"Плагины: фигур {lastPluginLoadResult.ShapePlugins}, обработка {lastPluginLoadResult.DataPlugins}";
        }

        private void UpdatePluginStatusLabel()
        {
            if (pluginStatusLabel != null)
                pluginStatusLabel.Text = GetPluginStatusText();
        }

        private void RefreshShapeTypeList()
        {
            string previouslySelected = shapeTypeCombo.SelectedItem?.ToString();
            shapeTypeCombo.Items.Clear();

            foreach (string shapeName in shapeCreator.GetAvailableShapes().OrderBy(s => s))
            {
                shapeTypeCombo.Items.Add(shapeName);
            }

            if (!string.IsNullOrWhiteSpace(previouslySelected) && shapeTypeCombo.Items.Contains(previouslySelected))
            {
                shapeTypeCombo.SelectedItem = previouslySelected;
            }
            else if (shapeTypeCombo.Items.Count > 0)
            {
                shapeTypeCombo.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Updates the shapes list box
        /// </summary>
        private void UpdateShapesList()
        {
            shapesListBox.Items.Clear();
            for (int i = 0; i < shapes.Count; i++)
            {
                shapesListBox.Items.Add($"{i + 1}: {shapes[i].Name}");
            }

            if (shapes.Count == 0)
            {
                ClearEditInputs();
            }
        }

        /// <summary>
        /// Handles shape creation event
        /// </summary>
        private void ShapeCreator_ShapeCreated(IShape shape)
        {
            shapes.AddShape(shape);
            UpdateShapesList();
            drawingPanel.Invalidate();
        }

        private Label CreateInputLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Location = location,
                Size = new Size(80, 20)
            };
        }

        private NumericUpDown CreateNumericInput(Point location)
        {
            return new NumericUpDown
            {
                Location = location,
                Size = new Size(80, 24),
                Minimum = -5000,
                Maximum = 5000
            };
        }

        private void SyncEditInputsFromSelectedShape()
        {
            var shape = GetSelectedShape();
            if (shape == null)
            {
                ClearEditInputs();
                return;
            }

            xInput.Value = ClampToNumeric(shape.Location.X);
            yInput.Value = ClampToNumeric(shape.Location.Y);
            widthInput.Value = ClampToNumeric(shape.Width);
            heightInput.Value = ClampToNumeric(shape.Height);
        }

        private void ClearEditInputs()
        {
            xInput.Value = 0;
            yInput.Value = 0;
            widthInput.Value = 0;
            heightInput.Value = 0;
        }

        private decimal ClampToNumeric(int value)
        {
            if (value < xInput.Minimum) return xInput.Minimum;
            if (value > xInput.Maximum) return xInput.Maximum;
            return value;
        }

        private IShape GetSelectedShape()
        {
            return shapesListBox.SelectedIndex >= 0 ? shapes[shapesListBox.SelectedIndex] : null;
        }

        private void ApplyChangesButton_Click(object sender, EventArgs e)
        {
            var shape = GetSelectedShape();
            if (shape == null)
            {
                MessageBox.Show("Select a shape first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            shape.MoveTo(new Point((int)xInput.Value, (int)yInput.Value));
            shape.Resize((int)widthInput.Value, (int)heightInput.Value);
            drawingPanel.Invalidate();
        }

        private void SaveXmlButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = dataProcessingPipeline.GetSaveFileFilter();
                dialog.DefaultExt = dataProcessingPipeline.GetDefaultExtension();
                dialog.FileName = "shapes." + dialog.DefaultExt;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    xmlService.Save(dialog.FileName, shapes);
                    MessageBox.Show(
                        $"Сохранено в {Path.GetFileName(dialog.FileName)} (формат: {dataProcessingPipeline.OutputFormat.ToUpper()}).",
                        "Сохранение",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private void LoadXmlButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = dataProcessingPipeline.GetLoadFileFilter();
                dialog.DefaultExt = "xml";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var loadedShapes = xmlService.Load(dialog.FileName);
                    shapes.Clear();
                    foreach (var shape in loadedShapes)
                    {
                        shapes.AddShape(shape);
                    }

                    UpdateShapesList();
                    drawingPanel.Invalidate();
                    MessageBox.Show(
                        $"Загружено фигур: {loadedShapes.Count} (формат после обработки: {dataProcessingPipeline.OutputFormat.ToUpper()}).",
                        "Загрузка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Handles drawing panel paint event
        /// </summary>
        private void DrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            shapeDrawer.DrawAllShapes(e.Graphics, shapes);
        }

        /// <summary>
        /// Handles mouse down on drawing panel
        /// </summary>
        private void DrawingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                shapeCreator.HandleMouseDown(e.Location);
            }
        }

        /// <summary>
        /// Handles mouse up on drawing panel
        /// </summary>
        private void DrawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                shapeCreator.HandleMouseUp(e.Location);
            }
        }
    }
}