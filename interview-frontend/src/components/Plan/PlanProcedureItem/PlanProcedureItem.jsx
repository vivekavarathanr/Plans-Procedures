import React, { useMemo } from "react";
import ReactSelect from "react-select";

const PlanProcedureItem = ({planProcedure, users, onAssignedUsersChange, onRemoveAllUsers }) => {

    const selectedUsers = useMemo(
        () => (planProcedure.assignedUsers || []).map((u) => ({
            label: u.user?.name,
            value: u.userId,
        })),
        [planProcedure.assignedUsers]
    );

    const handleAssignUsersToProcedure = async (selectedOptions) => {
        const nextUsers = selectedOptions || [];
        await onAssignedUsersChange(planProcedure, nextUsers);
    };

    return (
        <div className="py-2">
            <div className="d-flex justify-content-between align-items-center">
                <div>
                    {planProcedure.procedure.procedureTitle}
                </div>
                <button
                    className="btn btn-sm btn-outline-danger ms-2"
                    disabled={!selectedUsers.length}
                    onClick={() => onRemoveAllUsers(planProcedure)}
                >
                    X
                </button>
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={handleAssignUsersToProcedure}
            />
        </div>
    );
};

export default PlanProcedureItem;
