using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Base {
    public abstract class Action : Clickable {
        private ActionMetadata metadata;
        private Dictionary<string, ActionParameter> parameters = new Dictionary<string, ActionParameter>();
        public PuckInput Input;
        public PuckOutput Output;
        public IActionProvider ActionProvider;

        public IO.Swagger.Model.Action Data = new IO.Swagger.Model.Action("", new List<IO.Swagger.Model.ActionIO>(), new List<IO.Swagger.Model.ActionIO>(), new List<IO.Swagger.Model.ActionParameter>(), "");
        public void Init(string id, ActionMetadata metadata, ActionPoint ap, bool generateData, IActionProvider actionProvider, bool updateProject = true) {

            this.metadata = metadata;
            this.ActionProvider = actionProvider;

            if (generateData) {
                Debug.LogError(this.metadata);
                Debug.LogError(this.metadata.Parameters);
                foreach (IO.Swagger.Model.ActionParameterMeta actionParameterMetadata in this.metadata.Parameters) {
                    ActionParameter actionParameter = new ActionParameter(actionParameterMetadata);
                    switch (actionParameter.Type) {
                        case "relative_pose":
                            actionParameter.Value = Regex.Replace(new IO.Swagger.Model.Pose(orientation: new IO.Swagger.Model.Orientation(), position: new IO.Swagger.Model.Position()).ToJson(), @"\t|\n|\r", "");
                            break;
                        case "integer_enum":
                            actionParameter.Value = (int) actionParameterMetadata.AllowedValues[0];
                            break;
                        case "string_enum":
                            actionParameter.Value = (string) actionParameterMetadata.AllowedValues[0];
                            break;
                        case "pose":
                            List<string> poses = new List<string>(ap.GetPoses().Keys);
                            if (poses.Count == 0) {
                                actionParameter.Value = "";
                                //TODO: where to get valid ID?
                            } else {
                                actionParameter.Value = (string) ap.ActionObject.Data.Id + "." + ap.Data.Id + "." + poses[0];
                            }
                            break;
                        default:
                            actionParameter.Value = actionParameterMetadata.DefaultValue;
                            break;

                    }

                    Parameters[actionParameter.ActionParameterMetadata.Name] = actionParameter;
                }
                foreach (InputOutput io in GetComponentsInChildren<InputOutput>()) {
                    io.InitData();
                }
            }
            UpdateId(id, false);
            UpdateType();

            if (updateProject) {
                GameManager.Instance.UpdateProject();
            }


        }



        public void UpdateType() {
            Data.Type = GetActionType();
        }

        public virtual void UpdateId(string newId, bool updateProject = true) {
            Data.Id = newId;
            if (updateProject)
                GameManager.Instance.UpdateProject();
        }

        public string GetActionType() {
            return ActionProvider.GetProviderName() + "/" + metadata.Name; //TODO: AO|Service/Id
        }

        public void DeleteAction(bool updateProject = true) {
            foreach (InputOutput io in GetComponentsInChildren<InputOutput>()) {
                if (io.Connection != null)
                    Destroy(io.Connection.gameObject);
            }
            gameObject.SetActive(false);
            Destroy(gameObject);
            if (updateProject)
                GameManager.Instance.UpdateProject();
        }

        public Dictionary<string, ActionParameter> Parameters {
            get => parameters; set => parameters = value;
        }
        public ActionMetadata Metadata {
            get => metadata; set => metadata = value;
        }

    }

}
