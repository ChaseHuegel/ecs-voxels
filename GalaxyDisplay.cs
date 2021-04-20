using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swordfish;

public class Ellipsoid
{
    public Vector3[] points;

    public Ellipsoid()
    {

    }

    public Ellipsoid(Vector3[] _points)
    {
        points = _points;
    }
}

public class GalaxyDisplay : MonoBehaviour
{
    public bool generate = true;

    public int galaxyEllipseDetail = 64;
    public int galaxyEllipseCount = 10;
    public Vector3 galaxyEllipseCenter = Vector3.zero;

    public float galaxyEllipseRadiusX = 10.0f;
    public float galaxyEllipseRadiusY = 5.0f;
    public float galaxyEllipseThickness = 1.0f;
    public float galaxyGrowth = 1.0f;
    public float galaxyTiltGrowth = 1.0f;
    public float galaxyTilt = 1.0f;

    public AnimationCurve galaxyGrowthCurve;
    public AnimationCurve galaxyTiltCurve;

    public int starCount = 1000;
    public float starScale = 1.0f;
    public float starOffsetVariance = 1.0f;

    public Gradient starColor;
    public AnimationCurve starOffsetToDist;
    public AnimationCurve starBrightnessToDist;

    public Mesh starMesh;
    public Material starMaterial;

    public float dustScale = 1.0f;
    public float dustOffsetVariance = 1.0f;
    public Gradient dustColor;

    public Mesh dustMesh;
    public Material dustMaterial;

    public Shader glShader;
    private Material glMaterial;

    private Vector3[] stars;
    private Color[] starColors;
    private Matrix4x4[] starMatrices;
    private MaterialPropertyBlock starMaterialProperties;
    private Ellipsoid[] ellipses;

    private Vector3[] dust;
    private Color[] dustColors;
    private Vector4[] dustColorVectors;
    private Matrix4x4[] dustMatrices;
    private MaterialPropertyBlock dustMaterialProperties;

    private List<Unity.Entities.Entity> entities;

    public void Generate()
    {
        System.Console.WriteLine("Generating galaxy...");
        Random.InitState(Time.timeSinceLevelLoad.GetHashCode());
        galaxyEllipseCount = Random.Range(6, 24);
        galaxyEllipseRadiusX = Random.Range(2.0f, 16.0f);
        galaxyEllipseRadiusY = Random.Range(2.0f, 16.0f);
        galaxyGrowth = Random.Range(0.5f, 1.8f);
        galaxyTiltGrowth = Random.Range(10.0f, 120.0f);
        starOffsetVariance = Random.Range(16.0f, 100.0f);

        if (entities != null && entities.Count > 0)
        {
            foreach(Unity.Entities.Entity e in entities)
            {
                GameMaster.Instance.DestroyEntity(e);
            }
        }

        Unity.Rendering.RenderMesh starRenderer = new Unity.Rendering.RenderMesh { mesh = starMesh, material = starMaterial, castShadows = UnityEngine.Rendering.ShadowCastingMode.Off, receiveShadows = false };
        Unity.Rendering.RenderMesh dustRenderer = new Unity.Rendering.RenderMesh { mesh = dustMesh, material = dustMaterial, castShadows = UnityEngine.Rendering.ShadowCastingMode.Off, receiveShadows = false };

        entities = new List<Unity.Entities.Entity>();

        starMaterialProperties = new MaterialPropertyBlock();

        stars = new Vector3[starCount];
        starMatrices = new Matrix4x4[starCount];
        starColors = new Color[starCount];

        dustMaterialProperties = new MaterialPropertyBlock();

        dust = new Vector3[starCount];
        dustMatrices = new Matrix4x4[starCount];
        dustColors = new Color[starCount];
        dustColorVectors = new Vector4[starCount];

        ellipses = new Ellipsoid[galaxyEllipseCount];

        Vector3 offset;
        Vector3[] ellipsePoints;
        int starsPerEllipse = starCount / galaxyEllipseCount;
        int starsPerEllipsePoint = starsPerEllipse / galaxyEllipseDetail;
        int starIndex = 0;

        for (int i = 0; i < galaxyEllipseCount; i++)
        {
            ellipses[i] = new Ellipsoid(
                Util.CreateEllipse(
                    galaxyEllipseRadiusX * (galaxyGrowth * i),
                    galaxyEllipseRadiusY * (galaxyGrowth * i),
                    galaxyEllipseCenter.x,
                    galaxyEllipseCenter.y,
                    galaxyTilt + (galaxyTiltGrowth * i),
                    galaxyEllipseDetail
                )
            );
        }

        for (int i = 0; i < galaxyEllipseCount; i++)
        {
            ellipsePoints = ellipses[i].points;

            for (int n = 0; n < ellipsePoints.Length; n++)
            {
                for (int c = 0; c < starsPerEllipsePoint; c++)
                {
                    // offset = new Vector3(
                    //     Random.Range(-galaxyEllipseRadiusX, galaxyEllipseRadiusX),
                    //     Random.Range(-galaxyEllipseThickness, galaxyEllipseThickness),
                    //     Random.Range(-galaxyEllipseRadiusY, galaxyEllipseRadiusY)
                    // );

                    offset = Random.insideUnitSphere * Random.Range(-starOffsetVariance, starOffsetVariance);

                    starColors[starIndex] = starColor.Evaluate( Random.Range(0.0f, 1.0f) );

                    Vector3 curPoint = ellipses[i].points[n];
                    Vector3 prevPoint = ellipses[ Mathf.Clamp(i - 1, 0, galaxyEllipseCount - 1) ].points[n];
                    float value = Mathf.Clamp( 2.0f / ( Vector3.Distance(curPoint, prevPoint) ), 0.0f, 1.0f);
                    starColors[starIndex].a = starBrightnessToDist.Evaluate(value);

                    stars[starIndex] = ellipsePoints[n] + offset;
                    starMatrices[starIndex] = Matrix4x4.TRS(transform.position + stars[starIndex], Camera.main.transform.rotation, transform.lossyScale);

                    offset = Random.insideUnitSphere * Random.Range(-dustOffsetVariance, dustOffsetVariance);

                    dustColors[starIndex] = dustColor.Evaluate( Random.Range(0.0f, 1.0f) );
                    dustColorVectors[starIndex] = new Vector4(dustColors[starIndex].r, dustColors[starIndex].g, dustColors[starIndex].b, dustColors[starIndex].a);

                    dust[starIndex] = ellipsePoints[n] + offset;
                    dustMatrices[starIndex] = Matrix4x4.TRS(transform.position + dust[starIndex], Camera.main.transform.rotation, transform.lossyScale * dustScale);

                    Unity.Entities.Entity starEntity = GameMaster.Instance.CreateEntity(transform.position + stars[starIndex], Camera.main.transform.rotation, transform.lossyScale.x * starScale, starRenderer);
                    Unity.Entities.Entity dustEntity = GameMaster.Instance.CreateEntity(transform.position + dust[starIndex], Camera.main.transform.rotation, transform.lossyScale.x * dustScale, dustRenderer);

                    GameMaster.Instance.entityManager.AddComponentData(starEntity, new Swordfish.ecs.Billboard {});
                    GameMaster.Instance.entityManager.AddComponentData(dustEntity, new Swordfish.ecs.Billboard {});

                    // GameMaster.Instance.entityManager.AddComponentData(starEntity, new Swordfish.ecs.RenderColor { Value = starColors[starIndex] });
                    // GameMaster.Instance.entityManager.AddComponentData(dustEntity, new Swordfish.ecs.RenderColor { Value = dustColors[starIndex] });

                    entities.Add(starEntity);
                    entities.Add(dustEntity);

                    starIndex++;
                    if (starIndex >= starCount)
                    {
                        return;
                    }
                }
            }
        }
    }

    public void Start()
    {
        CreateLineMaterial();
    }

    public void Update()
    {
        if (generate)
        {
            Generate();
            generate = false;
        }
    }

    // public void LateUpdate()
    // {
    //     if (stars != null && stars.Length == starCount && generate == false)
    //     {
    //         Matrix4x4 matrix;
    //         for (int i = 0; i < starCount; i++)
    //         {
    //             Quaternion rot = Camera.main.transform.rotation;
    //             Random.InitState(i);
    //             Vector3 angles = rot.eulerAngles; angles.z += Random.Range(0, 360);
    //             rot = Quaternion.Euler(angles);

    //             starMaterialProperties.SetColor("_Color", starColors[i]);
    //             matrix = Matrix4x4.TRS(transform.position + stars[i], rot, transform.lossyScale);
    //             Graphics.DrawMesh(starMesh, matrix, starMaterial, 0, null, 0, starMaterialProperties, false, false, false);

    //             // dustMaterialProperties.SetColor("_Color", dustColors[i]);
    //             // matrix = Matrix4x4.TRS(transform.position + dust[i], rot, transform.lossyScale * dustScale);
    //             // Graphics.DrawMesh(dustMesh, matrix, starMaterial, 0, null, 0, dustMaterialProperties, false, false, false);
    //         }

    //         // dustMaterialProperties.SetVectorArray("_Color", dustColorVectors);
    //         // Graphics.DrawMeshInstanced(dustMesh, 0, dustMaterial, dustMatrices, starCount, dustMaterialProperties, UnityEngine.Rendering.ShadowCastingMode.Off, false);
    //     }
    // }

    public void CreateLineMaterial()
    {
        if( !glMaterial ) {
            glMaterial = new Material(glShader);
            glMaterial.hideFlags = HideFlags.HideAndDontSave;
            glMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    public void OnDrawGizmos()
    {
        if (stars != null && stars.Length == starCount && generate == false)
        {
            for (int i = 0; i < ellipses.Length; i++)
            {
                for (int n = 0; n < ellipses[i].points.Length; n++)
                {
                    Vector3 curPoint = ellipses[i].points[n];
                    Vector3 prevPoint = ellipses[i].points[ Mathf.Clamp(n - 1, 0, ellipses[i].points.Length - 1) ];

                    Debug.DrawLine(curPoint, prevPoint, Color.white, 0.0f, false);
                }
            }
        }
    }

    // public void OnPostRender()
    // {
    //     if (stars != null && stars.Length == starCount && generate == false)
    //     {
    //         GL.PushMatrix();
    //         GL.MultMatrix(transform.localToWorldMatrix);
    //         glMaterial.SetPass(0);

    //         GL.Begin(GL.LINES);
    //         GL.Color(Color.white);
    //         for (int i = 0; i < ellipses.Length; i++)
    //         {
    //             for (int n = 0; n < ellipses[i].points.Length; n++)
    //             {
    //                 GL.Vertex(ellipses[i].points[n]);
    //             }
    //         }
    //         GL.End();

    //         GL.PopMatrix();
    //     }
    // }
}
