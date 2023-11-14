using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using HIAAC.BehaviorTrees.Needs;

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Main BT Editor Windows class
    /// </summary>
    public class BehaviorTreeEditor : EditorWindow
    {
        BehaviorTreeView treeView; //Main tree view
        InspectorView inspectorView; //Lateral inspector view
        BlackboardView blackboardView; //Blackboard view over the treeView
        InspectorView agentParameters; //Lateral BTag parameters view
        NeedsView needsView;
        
        SerializedObject treeObject; //Active tree asset

        private VisualTreeAsset m_VisualTreeAsset; //UXML asset


        /// <summary>
        /// Creates option to open the editor in the menu.
        /// </summary>
        [MenuItem("Window/AI/Behavior Tree Editor")]
        public static void OpenWindow()
        {
            BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviorTreeEditor");
        }

        /// <summary>
        /// Creates the editor
        /// </summary>
        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            
            //Get the UXML object
            if(m_VisualTreeAsset == null)
            {
                string[] guidsVisualTree = AssetDatabase.FindAssets("t:VisualTreeAsset BehaviorTreeEditor");
                m_VisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guidsVisualTree[0]));
            }

            // Instantiate UXML
            m_VisualTreeAsset.CloneTree(root);

            //Gets the USS
            string[] guids = AssetDatabase.FindAssets("t:StyleSheet BehaviorTreeEditor");
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(guids[0]));
            root.styleSheets.Add(styleSheet);

            //Get the reference to each editor piece
            treeView = root.Q<BehaviorTreeView>();
            inspectorView = root.Q<InspectorView>("inspector");
            agentParameters = root.Q<InspectorView>("agent-parameters");
            needsView = root.Q<NeedsView>();
            GenerateBlackboard();

            //Define the delegate methods
            treeView.OnNodeSelected = (node) => { OnElementSelectionChanged(node.node); };
            blackboardView.OnPropertySelect = OnElementSelectionChanged;

            //Update the view
            OnSelectionChange();
        }

        /// <summary>
        /// Create and configure blackboard view
        /// </summary>
        void GenerateBlackboard()
        {
            blackboardView = new BlackboardView(treeView);

            treeView.Add(blackboardView);
            treeView.blackboard = blackboardView;
        }

        /// <summary>
        /// Update the tree view each frame
        /// </summary>
        void OnInspectorUpdate()
        {
            treeView.UpdateNodeStates();

            //REMOVE
            needsView.OnInspectorUpdate();
        }

        
        /// <summary>
        /// Update the inspector view if the selected element changed.
        /// </summary>
        /// <param name="obj">Selected element (Node or BlackboardProperty) </param>
        void OnElementSelectionChanged(UnityEngine.Object obj)
        {
            inspectorView.UpdateSelection(obj);
        }

        void OnElementSelectionChanged(SerializedProperty obj)
        {
            inspectorView.UpdateSelection(obj);
        }


        /// <summary>
        /// Update active tree if avaiable
        /// </summary>
        void OnSelectionChange()
        {  
            //Clear inspector view
            if (inspectorView != null)
            {
                inspectorView.Clear();
            }


            //Get tree from selection
            BehaviorTree tree = Selection.activeObject as BehaviorTree;

            //If tree not selected, check if Game Object have tree in BehaviorTreeRunner
            if (!tree && Selection.activeGameObject)
            {
                BehaviorTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>();

                if (runner)
                {
                    tree = runner.tree;
                }
            }

            //No tree -> return
            if (!tree)
            {
                return;
            }

            //Validate asset and get serialized
            tree.Validate();
            treeObject = new SerializedObject(tree);

            //Update tree view
            if (treeView != null)
            {
                if (Application.isPlaying || AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())) //Check if valid (asset or play mode)
                {
                    treeView.PopulateView(tree);
                    blackboardView.PopulateView(tree);
                    agentParameters.UpdateSelection(treeObject.FindProperty("bTagParameters"));
                }
            }
        }


        /// <summary>
        /// Open editor if opened asset is Behavior Tree
        /// </summary>
        /// <param name="instanceId">Not used</param>
        /// <param name="line">Not used</param>
        /// <returns>True if the asset is BT</returns>
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is BehaviorTree)
            {
                OpenWindow();
                return true;
            }
            return false;
        }


        // Change active tree on switch Editor/Play mode //--------------------------------------

        /// <summary>
        /// Add play mode changed event callback
        /// </summary>
        void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// Remove play mode changed callback
        /// </summary>
        void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }


        /// <summary>
        /// Update the selection when entering new mode
        /// </summary>
        /// <param name="change">Change to check if entered in new mode</param>
        void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;

                case PlayModeStateChange.ExitingEditMode:
                    break;

                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }

        }
    }
}