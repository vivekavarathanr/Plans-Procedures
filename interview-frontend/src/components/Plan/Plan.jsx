import React, { useMemo, useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import {
  addProcedureToPlan,
  assignUserToPlanProcedure,
  getPlanProcedures,
  getProcedures,
  getUsers,
  removeAllUsersFromPlanProcedure,
  removeUserFromPlanProcedure,
} from "../../api/api";
import Layout from '../Layout/Layout';
import ProcedureItem from "./ProcedureItem/ProcedureItem";
import PlanProcedureItem from "./PlanProcedureItem/PlanProcedureItem";

const Plan = () => {
  let { id } = useParams();
  const [procedures, setProcedures] = useState([]);
  const [planProcedures, setPlanProcedures] = useState([]);
  const [users, setUsers] = useState([]);

  const userOptions = useMemo(
    () => users.map((u) => ({ label: u.name, value: u.userId })),
    [users]
  );

  const usersById = useMemo(
    () => users.reduce((acc, user) => ({ ...acc, [user.userId]: user }), {}),
    [users]
  );

  useEffect(() => {
    (async () => {
      var users = await getUsers();

      var userOptions = [];
      users.map((u) => userOptions.push({ label: u.name, value: u.userId }));
      var proceduresData = await getProcedures();
      var planProceduresData = await getPlanProcedures(id);
      var usersData = await getUsers();

      setUsers(usersData);
      setProcedures(proceduresData);
      setPlanProcedures(planProceduresData);
    })();
  }, [id]);

  const handleAddProcedureToPlan = async (procedure) => {
    const hasProcedureInPlan = planProcedures.some((p) => p.procedureId === procedure.procedureId);
    if (hasProcedureInPlan) return;

    await addProcedureToPlan(id, procedure.procedureId);
    setPlanProcedures((prevState) => {
      return [
        ...prevState,
        {
          planId: id,
          procedureId: procedure.procedureId,
          procedure: {
            procedureId: procedure.procedureId,
            procedureTitle: procedure.procedureTitle,
          },
          assignedUsers: [],
        },
      ];
    });
  };

  const handleAssignedUsersChange = async (planProcedure, nextSelectedUsers) => {
    const currentAssignedUsers = planProcedure.assignedUsers || [];

    const currentIds = currentAssignedUsers.map((u) => u.userId);
    const nextIds = nextSelectedUsers.map((u) => u.value);

    const addedIds = nextIds.filter((userId) => !currentIds.includes(userId));
    const removedIds = currentIds.filter((userId) => !nextIds.includes(userId));

    if (nextIds.length === 0) {
      await removeAllUsersFromPlanProcedure(planProcedure.planId, planProcedure.procedureId);
    }
    else{
      if (addedIds.length > 0) {
        await Promise.all(
          addedIds.map((userId) => assignUserToPlanProcedure(planProcedure.planId, planProcedure.procedureId, userId))
        );
      }

      if (removedIds.length > 0) {
        await Promise.all(
          removedIds.map((userId) => removeUserFromPlanProcedure(planProcedure.planId, planProcedure.procedureId, userId))
        );
      }
        }

    setPlanProcedures((prevState) => prevState.map((pp) => {
      if (pp.planId !== planProcedure.planId || pp.procedureId !== planProcedure.procedureId) {
        return pp;
      }

      return {
        ...pp,
        assignedUsers: nextIds.map((userId) => ({
          planId: pp.planId,
          procedureId: pp.procedureId,
          userId,
          user: usersById[userId],
        })),
      };
    }));
  };

  return (
    <Layout>
      <div className="container pt-4">
        <div className="d-flex justify-content-center">
          <h2>OEC Interview Frontend</h2>
        </div>
        <div className="row mt-4">
          <div className="col">
            <div className="card shadow">
              <h5 className="card-header">Repair Plan</h5>
              <div className="card-body">
                <div className="row">
                  <div className="col">
                    <h4>Procedures</h4>
                    <div>
                      {procedures.map((p) => (
                        <ProcedureItem
                          key={p.procedureId}
                          procedure={p}
                          handleAddProcedureToPlan={handleAddProcedureToPlan}
                          planProcedures={planProcedures}
                        />
                      ))}
                    </div>
                  </div>
                  <div className="col">
                    <h4>Added to Plan</h4>
                    <div>
                      {planProcedures.map((p) => (
                        <PlanProcedureItem
                          key={p.procedure.procedureId}
                          procedure={p.procedure}
                          planProcedure={p}
                          users={userOptions}
                          onAssignedUsersChange={handleAssignedUsersChange}
                        />
                      ))}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default Plan;