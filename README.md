# SectionProperties

Users can build up beam sections using common shapes (rectangles, triangles, angle sections, channel sections, etc).

Sections are constructed using shapes and even other sections.

<p align="center" style="font-size:160%;"><b>Tree View Visualization of Section Build up</b></p>

<p align="center">
  <img src="https://i.imgur.com/cN11613.png" width="500" align="middle">
</p>

<h2>Materials</h2>

Users define materials, and then assign them to a shape prior to building up a section. The following properties can be defined:

<ul style="list-style: none;">
  <li>E: Modulus of Elasticity</li>
  <li>G: Shear Modulus</li>
  <li>v: Poisson's ratio</li>
  <li>MaterialColor_Hex: color for all shapes composed of this color (defined in HEX)</li>
</ul>

Currently, only isotropic materials can be used. Orthotropic materials have been explored, but haven't been fully implemented yet.

Example: 

<code>
  IsoMaterial mat = new IsoMaterial { Name = "mat1", E = 1, G = 1, v = 0.33, MaterialColor_Hex = "#00FF00" };
</code>

<p><br></p>

<h2>Shapes</h2>

Positioning: 
Position shapes by defining a point (specific to each shape type, shown below) and it's location coordinates, xp and yp. 

<p align="center">
  <img src="https://i.imgur.com/fNhdeA9.png" width="500" align="middle">
</p>

Dimensions:
Each shape has a unique set of dimensions that can be defined.

Rotation:
Rotate shapes using the ".theta" property

Mirroring:
Mirror shapes using the ".mirrorX" and ".mirrorY" properties

Example:

<pre><code>
  SectionElements.MachinedAngle MA = new SectionElements.MachinedAngle();
            MA.b1 = 4;
            MA.b2 = 5;
            MA.t1 = 0.50;
            MA.t2 = 0.50;
            MA.r = 0.50;
            MA.theta = 30 * Math.PI / 180;
            MA.point = "a";
            MA.xp = 1.6824;
            MA.yp = -0.1334;
            MA.Material = mat;
            MA.mirrorY = true;
</code></pre>

<h2>Sections</h2>

Add shapes to section:

<pre><code>
  Section sec = new Section();

            sec.AddShape(MA);
</code></pre>

Calculate Section Properties (EA, EIxx, EIyy, EIxy, Xcg, Ycg, etc)

<pre><code>
  SecProp sp = sec.CalculateSecProp();
</code></pre>

Draw Section to file:

<pre><code>
  EnvelopeCoords env = sec.GetEnvelopeCoord(); //GetEnvelopeCoords will find the x and y min and max of the section (for plotting purposes)

  Bitmap bitmap = new Bitmap(1000, 1000);
  PlotProperties pp = env.GetPlotProperties(1); //adds padding to envelope from above

  sec.Draw(ref bitmap, pp);

  bitmap.Save("Shape2.bmp");
</code></pre>


<h2>Full Sample</h2>

<pre><code>
            IsoMaterial mat = new IsoMaterial { Name = "mat1", E = 1, G = 1, v = 0.33, MaterialColor_Hex = "#00FF00" };

            SectionElements.MachinedAngle MA = new SectionElements.MachinedAngle();
            MA.b1 = 4;
            MA.b2 = 5;
            MA.t1 = 0.50;
            MA.t2 = 0.50;
            MA.r = 0.50;
            MA.theta = 30 * Math.PI / 180;
            MA.point = "a";
            MA.xp = 1.6824;
            MA.yp = -0.1334;
            MA.Material = mat;
            MA.mirrorY = true;

            SectionElements.Rectangle rec = new SectionElements.Rectangle();
            rec.b = 6;
            rec.t = 0.375;
            rec.point = "g";
            rec.xp = 2.7359;
            rec.yp = 0.0369;
            rec.theta = MA.theta;
            rec.Material = mat;

            Section sec = new Section();

            sec.AddShape(MA);
            sec.AddShape(rec);
            SecProp sp = sec.CalculateSecProp();

            EnvelopeCoords env = sec.GetEnvelopeCoord();

            Bitmap bitmap = new Bitmap(1000, 1000);
            PlotProperties pp = env.GetPlotProperties(1);

            sec.Draw(ref bitmap, pp);

            bitmap.Save("Shape2.bmp");
</code></pre>

<p align="center">
  <img src="https://i.imgur.com/pLVo4Ec.png" width="250" align="middle">
  <br>
  <b>sec.Draw(...) output<b>
</p>
