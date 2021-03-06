#!/bin/bash
mkdir -p "${STEAMAPPDIR}" || true

# Override SteamCMD launch arguments if necessary
# Used for subscribing to betas or for testing
if [ -z "$STEAMCMD_UPDATE_ARGS" ]; then
	bash "${STEAMCMDDIR}/steamcmd.sh" +login anonymous +force_install_dir "$STEAMAPPDIR" +app_update "$STEAMAPPID" +quit
else
	steamcmd_update_args=($STEAMCMD_UPDATE_ARGS)
	bash "${STEAMCMDDIR}/steamcmd.sh" +login anonymous +force_install_dir "$STEAMAPPDIR" +app_update "$STEAMAPPID" "${steamcmd_update_args[@]}" +quit
fi

# Replace pre-set variables
#sed -i -e 's/server_port 20100/'"server_port ${SERVER_PORT}"'/g' \
#		-e 's/server_region europe/'"server_region ${SERVER_REGION}"'/g' \
#		-e 's/steam_communications_port 8700/'"steam_communications_port ${STEAM_COM_PORT}"'/g' \
#		-e 's/steam_query_port 27000/'"steam_query_port ${STEAM_QUERY_PORT}"'/g' \
#			"${STEAMAPPDIR}/${SERVER_CONFIG_PATH}"

cd "${STEAMAPPDIR}"
"./run_bepinex.sh"
